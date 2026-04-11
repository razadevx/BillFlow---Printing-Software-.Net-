using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;

namespace BillFlow.ViewModels;

public partial class CustomerDetailViewModel : ViewModelBase
{
    private readonly ICustomerService _customerService;
    private readonly IKhataService _khataService;
    private readonly ICalculationService _calculationService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Customer? _customer;

    [ObservableProperty]
    private ObservableCollection<WorkOrder> _workOrders = new();

    [ObservableProperty]
    private ObservableCollection<KhataEntry> _allKhataEntries = new();

    [ObservableProperty]
    private ObservableCollection<KhataEntry> _paymentKhataEntries = new();

    [ObservableProperty]
    private decimal _todayPending;

    [ObservableProperty]
    private CreditRiskLevel _riskLevel;

    [ObservableProperty]
    private string _riskEmoji = "🟢";

    public CustomerDetailViewModel(
        ICustomerService customerService,
        IKhataService khataService,
        ICalculationService calculationService,
        INavigationService navigationService)
    {
        _customerService = customerService;
        _khataService = khataService;
        _calculationService = calculationService;
        _navigationService = navigationService;
        PageTitle = "Customer";
    }

    public async Task LoadAsync(int customerId)
    {
        IsLoading = true;
        ClearMessages();
        try
        {
            var c = await _customerService.GetByIdAsync(customerId);
            Customer = c;
            if (c == null)
            {
                ShowError("Customer not found");
                return;
            }

            PageTitle = c.Name;
            WorkOrders = new ObservableCollection<WorkOrder>(
                c.WorkOrders.OrderByDescending(w => w.OrderDate));

            var entries = await _khataService.GetEntriesByCustomerAsync(customerId);
            AllKhataEntries = new ObservableCollection<KhataEntry>(entries);
            PaymentKhataEntries = new ObservableCollection<KhataEntry>(
                entries.Where(e => e.Type == TransactionType.Debit).OrderByDescending(e => e.TransactionDate));

            var today = DateTime.Today;
            TodayPending = c.WorkOrders
                .Where(w => w.ScheduledDate?.Date == today && w.PendingAmount > 0)
                .Sum(w => w.PendingAmount);

            RiskLevel = _calculationService.GetCreditRisk(c, c.LastPaymentDate);
            RiskEmoji = RiskLevel switch
            {
                CreditRiskLevel.Clear => "🟢",
                CreditRiskLevel.Moderate => "🟡",
                CreditRiskLevel.HighRisk => "🔴",
                _ => "⚪"
            };
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void BackToCustomers()
    {
        _navigationService.NavigateTo("Customers");
    }
}
