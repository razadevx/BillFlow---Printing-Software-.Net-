using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;

namespace BillFlow.ViewModels;

public sealed class DashboardSchedulePreviewRow
{
    public required WorkOrder Order { get; init; }
    public required string Title { get; init; }
    public required string DetailLine { get; init; }
}

public partial class DashboardViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IKhataService _khataService;
    private readonly IWorkOrderService _workOrderService;

    [ObservableProperty]
    private decimal _totalPendingAmount;

    [ObservableProperty]
    private decimal _todayRevenue;

    [ObservableProperty]
    private int _todayWorkDone;

    [ObservableProperty]
    private int _todayWorkTotal;

    [ObservableProperty]
    private int _activeCustomers;

    [ObservableProperty]
    private ObservableCollection<CustomerCreditSummary> _topCustomers = new();

    [ObservableProperty]
    private ObservableCollection<DashboardSchedulePreviewRow> _todaySchedulePreview = new();

    public int ProgressBarMaximum => Math.Max(TodayWorkTotal, 1);

    public DashboardViewModel(
        INavigationService navigationService,
        IKhataService khataService,
        IWorkOrderService workOrderService)
    {
        _navigationService = navigationService;
        _khataService = khataService;
        _workOrderService = workOrderService;
        PageTitle = "Dashboard";

        _ = LoadDashboardDataAsync();
    }

    [RelayCommand]
    public async Task LoadDashboardDataAsync()
    {
        IsLoading = true;

        try
        {
            var creditSummary = await _khataService.GetAllCustomersCreditSummaryAsync();
            TotalPendingAmount = creditSummary.Sum(c => c.TotalCredit);
            ActiveCustomers = creditSummary.Count(c => c.TotalCredit > 0);
            var withOutstanding = creditSummary.Where(c => c.TotalCredit > 0).Take(12).ToList();
            TopCustomers = new ObservableCollection<CustomerCreditSummary>(withOutstanding);

            TodayRevenue = await _khataService.GetTodayPaymentsTotalAsync();

            var todayWork = await _workOrderService.GetByScheduledDateAsync(DateTime.Today);
            TodayWorkTotal = todayWork.Count;
            TodayWorkDone = todayWork.Count(w =>
                w.WorkStatus == WorkStatus.Completed || w.WorkStatus == WorkStatus.Delivered);

            var rows = todayWork.Take(8).Select(w =>
            {
                var desc = w.LineItems.FirstOrDefault()?.Description;
                if (string.IsNullOrWhiteSpace(desc))
                    desc = string.IsNullOrWhiteSpace(w.Notes) ? "Work order" : w.Notes!;
                return new DashboardSchedulePreviewRow
                {
                    Order = w,
                    Title = $"{w.Customer.Name} — {desc}",
                    DetailLine = $"{w.TotalArea:F1} SqFt • PKR {w.GrandTotal:N2}"
                };
            }).ToList();
            TodaySchedulePreview = new ObservableCollection<DashboardSchedulePreviewRow>(rows);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load dashboard: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(ProgressBarMaximum));
        }
    }

    partial void OnTodayWorkTotalChanged(int value) => OnPropertyChanged(nameof(ProgressBarMaximum));

    partial void OnTodayWorkDoneChanged(int value) => OnPropertyChanged(nameof(ProgressBarMaximum));

    [RelayCommand]
    private async Task MarkScheduleItemDoneAsync(DashboardSchedulePreviewRow? row)
    {
        if (row == null) return;
        try
        {
            await _workOrderService.UpdateStatusAsync(row.Order.Id, WorkStatus.Completed);
            await LoadDashboardDataAsync();
            ShowSuccess("Marked as completed");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private void NavigateToDailySchedule()
    {
        _navigationService.NavigateTo("DailySchedule");
    }

    [RelayCommand]
    private void NavigateToKhata()
    {
        _navigationService.NavigateTo("KhataLedger");
    }

    [RelayCommand]
    private void QuickAddCustomer()
    {
        _navigationService.NavigateTo("Customers");
    }

    [RelayCommand]
    private void QuickCreateWorkOrder()
    {
        _navigationService.NavigateTo("WorkOrderCreate");
    }

    [RelayCommand]
    private void RecordPayment()
    {
        _navigationService.NavigateTo("KhataLedger");
    }
}
