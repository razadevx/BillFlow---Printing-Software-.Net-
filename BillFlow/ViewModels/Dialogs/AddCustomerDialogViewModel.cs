using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;

namespace BillFlow.ViewModels.Dialogs;

public partial class AddCustomerDialogViewModel : ObservableValidator
{
    private readonly ICustomerService _customerService;
    private int? _editingCustomerId;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Customer name is required.")]
    [MinLength(2, ErrorMessage = "Name must be at least 2 characters.")]
    private string _customerName = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RegularExpression(@"^[\d\s\+\-]+$", ErrorMessage = "Phone can only include digits, spaces, + and -.")]
    private string _phone = string.Empty;

    [ObservableProperty]
    private string _address = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, double.MaxValue, ErrorMessage = "Credit limit cannot be negative.")]
    private decimal _creditLimit = 10000m;

    [ObservableProperty]
    private string? _errorMessage;

    public event EventHandler<bool>? RequestClose;

    public string DialogTitle => _editingCustomerId.HasValue ? "Edit Customer" : "Add New Customer";
    public string SaveButtonText => _editingCustomerId.HasValue ? "Update Customer" : "Save Customer";
    
    public bool CanSave => !HasErrors && !string.IsNullOrWhiteSpace(CustomerName) && CustomerName.Trim().Length >= 2;

    public AddCustomerDialogViewModel(ICustomerService customerService)
    {
        _customerService = customerService;
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(HasErrors) || e.PropertyName == nameof(CustomerName) || e.PropertyName == nameof(CreditLimit))
            {
                OnPropertyChanged(nameof(CanSave));
                SaveCommand.NotifyCanExecuteChanged();
            }
        };
        ErrorsChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(CanSave));
            SaveCommand.NotifyCanExecuteChanged();
        };
    }

    partial void OnCustomerNameChanged(string value)
    {
        ErrorMessage = null;
    }

    partial void OnCreditLimitChanged(decimal value)
    {
        ErrorMessage = null;
    }

    partial void OnPhoneChanged(string value)
    {
        ErrorMessage = null;
    }

    public void InitializeForCreate()
    {
        _editingCustomerId = null;
        CustomerName = string.Empty;
        Phone = string.Empty;
        Address = string.Empty;
        CreditLimit = 10000m;
        ErrorMessage = null;

        ClearErrors();

        OnPropertyChanged(nameof(DialogTitle));
        OnPropertyChanged(nameof(SaveButtonText));
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    public void InitializeForEdit(Customer customer)
    {
        _editingCustomerId = customer.Id;
        CustomerName = customer.Name;
        Phone = customer.Phone ?? string.Empty;
        Address = customer.Address ?? string.Empty;
        CreditLimit = customer.CreditLimit;
        ErrorMessage = null;
        ClearErrors();
        ValidateAllProperties();
        
        OnPropertyChanged(nameof(DialogTitle));
        OnPropertyChanged(nameof(SaveButtonText));
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        ValidateAllProperties();
        if (HasErrors) return;

        try
        {
            if (_editingCustomerId.HasValue)
            {
                var existingCustomer = await _customerService.GetByIdAsync(_editingCustomerId.Value);
                if (existingCustomer == null)
                {
                    ErrorMessage = "Customer not found.";
                    return;
                }

                existingCustomer.Name = CustomerName.Trim();
                existingCustomer.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();
                existingCustomer.Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim();
                existingCustomer.CreditLimit = CreditLimit;

                await _customerService.UpdateAsync(existingCustomer);
            }
            else
            {
                var customer = new Customer
                {
                    Name = CustomerName.Trim(),
                    Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                    Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
                    CreditLimit = CreditLimit
                };

                await _customerService.CreateAsync(customer);
            }

            RequestClose?.Invoke(this, true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
