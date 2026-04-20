using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;

namespace BillFlow.ViewModels;

public partial class KhataLedgerViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IKhataService _khataService;
    private readonly ICustomerService _customerService;

    [ObservableProperty]
    private ObservableCollection<CustomerCreditSummary> _customerSummaries = new();

    [ObservableProperty]
    private CustomerCreditSummary? _selectedSummary;

    [ObservableProperty]
    private ObservableCollection<KhataEntry> _khataEntries = new();

    [ObservableProperty]
    private decimal _totalPending;

    [ObservableProperty]
    private decimal _totalPaidToday;

    [ObservableProperty]
    private bool _showRecordPaymentDialog;

    public KhataLedgerViewModel(
        INavigationService navigationService, 
        IKhataService khataService,
        ICustomerService customerService)
    {
        _navigationService = navigationService;
        _khataService = khataService;
        _customerService = customerService;
        PageTitle = "Khata Ledger";
        
        _ = LoadKhataSummaryAsync();
    }

    [RelayCommand]
    public async Task LoadKhataSummaryAsync()
    {
        IsLoading = true;
        ClearMessages();
        
        try
        {
            var summaries = await _khataService.GetAllCustomersCreditSummaryAsync();
            CustomerSummaries = new ObservableCollection<CustomerCreditSummary>(summaries);
            TotalPending = summaries.Sum(s => s.TotalCredit);
            TotalPaidToday = await _khataService.GetTodayPaymentsTotalAsync();
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load khata summary: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void RecordPayment(CustomerCreditSummary? summary)
    {
        summary ??= SelectedSummary;
        if (summary == null) return;
        SelectedSummary = summary;
        ShowRecordPaymentDialog = true;
    }

    [RelayCommand]
    private async Task ViewCustomerKhataAsync(CustomerCreditSummary summary)
    {
        if (summary == null) return;
        
        SelectedSummary = summary;
        IsLoading = true;
        
        try
        {
            var entries = await _khataService.GetEntriesByCustomerAsync(summary.Customer.Id);
            KhataEntries = new ObservableCollection<KhataEntry>(
                entries.OrderByDescending(e => e.TransactionDate).ThenByDescending(e => e.Id));
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load khata entries: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
