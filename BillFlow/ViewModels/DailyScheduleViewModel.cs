using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;
using BillFlow.Views.Dialogs;

namespace BillFlow.ViewModels;

public partial class DailyScheduleViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IWorkOrderService _workOrderService;

    [ObservableProperty]
    private DateTime _selectedDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<WorkOrder> _completedWork = new();

    [ObservableProperty]
    private ObservableCollection<WorkOrder> _pendingWork = new();

    [ObservableProperty]
    private decimal _totalSqFt;

    [ObservableProperty]
    private decimal _totalRevenue;

    [ObservableProperty]
    private int _doneCount;

    [ObservableProperty]
    private int _todoCount;

    public DailyScheduleViewModel(INavigationService navigationService, IWorkOrderService workOrderService)
    {
        _navigationService = navigationService;
        _workOrderService = workOrderService;
        PageTitle = "Daily Schedule";
        _ = LoadScheduleAsync();
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        _ = LoadScheduleAsync();
    }

    private static bool IsDone(WorkOrder w) =>
        w.WorkStatus == WorkStatus.Completed || w.WorkStatus == WorkStatus.Delivered;

    [RelayCommand]
    public async Task LoadScheduleAsync()
    {
        IsLoading = true;
        try
        {
            var orders = await _workOrderService.GetByScheduledDateAsync(SelectedDate);
            var done = orders.Where(IsDone).OrderBy(w => w.Customer.Name).ToList();
            var todo = orders.Where(w => !IsDone(w)).OrderBy(w => w.Customer.Name).ToList();

            CompletedWork = new ObservableCollection<WorkOrder>(done);
            PendingWork = new ObservableCollection<WorkOrder>(todo);

            DoneCount = done.Count;
            TodoCount = todo.Count;
            TotalSqFt = orders.Sum(o => o.TotalArea);
            TotalRevenue = done.Sum(o => o.AmountPaid);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load schedule: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task MarkAsDoneAsync(WorkOrder? workOrder)
    {
        if (workOrder == null) return;
        try
        {
            await _workOrderService.UpdateStatusAsync(workOrder.Id, WorkStatus.Completed);
            await LoadScheduleAsync();
            ShowSuccess("Marked as completed");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private async Task RescheduleAsync(WorkOrder? workOrder)
    {
        if (workOrder == null) return;
        var dlg = new RescheduleDateDialog(workOrder.ScheduledDate ?? SelectedDate);
        if (dlg.ShowDialog() != true) return;

        try
        {
            await _workOrderService.UpdateScheduledDateAsync(workOrder.Id, dlg.SelectedDate);
            await LoadScheduleAsync();
            ShowSuccess("Date updated");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    [RelayCommand]
    private void ViewDetails(WorkOrder? workOrder)
    {
        if (workOrder == null) return;
        _navigationService.NavigateToCustomerDetail(workOrder.CustomerId);
    }

    [RelayCommand]
    private void PreviousDay()
    {
        SelectedDate = SelectedDate.AddDays(-1);
    }

    [RelayCommand]
    private void NextDay()
    {
        SelectedDate = SelectedDate.AddDays(1);
    }
}
