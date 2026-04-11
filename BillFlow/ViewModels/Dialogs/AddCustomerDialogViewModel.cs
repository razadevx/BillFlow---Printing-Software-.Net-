using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;

namespace BillFlow.ViewModels.Dialogs;

public partial class AddCustomerDialogViewModel : ObservableObject
{
    private readonly ICustomerService _customerService;

    [ObservableProperty]
    private string _customerName = string.Empty;

    [ObservableProperty]
    private string _phone = string.Empty;

    [ObservableProperty]
    private string _address = string.Empty;

    [ObservableProperty]
    private decimal _creditLimit = 10000m;

    [ObservableProperty]
    private string? _errorMessage;

    public event EventHandler<bool>? RequestClose;

    public bool CanSave => !string.IsNullOrWhiteSpace(CustomerName) && CustomerName.Length >= 2;

    public AddCustomerDialogViewModel(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    partial void OnCustomerNameChanged(string value)
    {
        ErrorMessage = null;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()
    {
        try
        {
            var customer = new Customer
            {
                Name = CustomerName.Trim(),
                Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim(),
                Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
                CreditLimit = CreditLimit
            };

            await _customerService.CreateAsync(customer);
            RequestClose?.Invoke(this, true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
