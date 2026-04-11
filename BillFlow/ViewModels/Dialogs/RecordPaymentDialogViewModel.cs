using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;

namespace BillFlow.ViewModels.Dialogs;

public partial class RecordPaymentDialogViewModel : ObservableObject
{
    private readonly IKhataService _khataService;
    private readonly IWorkOrderService _workOrderService;

    [ObservableProperty]
    private string _customerName = string.Empty;

    [ObservableProperty]
    private int _customerId;

    [ObservableProperty]
    private decimal _currentBalance;

    [ObservableProperty]
    private decimal _paymentAmount;

    [ObservableProperty]
    private string _selectedPaymentMethod = "Cash";

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    public List<string> PaymentMethods => new() { "Cash", "UPI", "Bank Transfer", "Cheque" };

    public event EventHandler<bool>? RequestClose;

    public bool CanRecord => PaymentAmount > 0 && PaymentAmount <= CurrentBalance;

    public RecordPaymentDialogViewModel(IKhataService khataService, IWorkOrderService workOrderService)
    {
        _khataService = khataService;
        _workOrderService = workOrderService;
    }

    partial void OnPaymentAmountChanged(decimal value)
    {
        ErrorMessage = null;
        if (value > CurrentBalance)
        {
            ErrorMessage = "Payment amount cannot exceed current balance";
        }
    }

    public void SetCustomer(Customer customer)
    {
        CustomerId = customer.Id;
        CustomerName = customer.Name;
        CurrentBalance = customer.TotalCredit;
    }

    [RelayCommand(CanExecute = nameof(CanRecord))]
    private async Task RecordPaymentAsync()
    {
        try
        {
            await _khataService.AddDebitEntryAsync(
                CustomerId, 
                PaymentAmount, 
                $"Payment via {SelectedPaymentMethod}. {Notes}",
                null);

            RequestClose?.Invoke(this, true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}
