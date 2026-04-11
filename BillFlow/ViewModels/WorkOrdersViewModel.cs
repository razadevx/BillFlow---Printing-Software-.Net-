using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;

namespace BillFlow.ViewModels;

public partial class WorkOrdersViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IWorkOrderService _workOrderService;

    [ObservableProperty]
    private ObservableCollection<WorkOrder> _workOrders = new();

    [ObservableProperty]
    private WorkOrder? _selectedWorkOrder;

    [ObservableProperty]
    private WorkStatus? _selectedStatusFilter = null;

    [ObservableProperty]
    private int _totalWorkOrders;

    [ObservableProperty]
    private int _pendingWorkOrders;

    [ObservableProperty]
    private int _completedToday;

    partial void OnSelectedStatusFilterChanged(WorkStatus? value)
    {
        _ = FilterWorkOrdersAsync();
    }

    public WorkOrdersViewModel(INavigationService navigationService, IWorkOrderService workOrderService)
    {
        _navigationService = navigationService;
        _workOrderService = workOrderService;
        PageTitle = "Work Orders";
        
        _ = LoadWorkOrdersAsync();
    }

    [RelayCommand]
    private async Task LoadWorkOrdersAsync()
    {
        IsLoading = true;
        ClearMessages();
        
        try
        {
            List<WorkOrder> orders;
            
            if (SelectedStatusFilter.HasValue)
            {
                orders = await _workOrderService.GetByStatusAsync(SelectedStatusFilter.Value);
            }
            else
            {
                orders = await _workOrderService.GetAllAsync();
            }
            
            WorkOrders = new ObservableCollection<WorkOrder>(orders);
            
            // Update stats
            TotalWorkOrders = orders.Count;
            PendingWorkOrders = orders.Count(o => o.WorkStatus != WorkStatus.Completed && o.WorkStatus != WorkStatus.Delivered);
            CompletedToday = orders.Count(o => o.WorkStatus == WorkStatus.Completed && o.OrderDate.Date == DateTime.Today);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load work orders: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task FilterWorkOrdersAsync()
    {
        await LoadWorkOrdersAsync();
    }

    [RelayCommand]
    private void CreateWorkOrder()
    {
        _navigationService.NavigateTo("WorkOrderCreate");
    }

    [RelayCommand]
    private void EditWorkOrder(WorkOrder workOrder)
    {
        SelectedWorkOrder = workOrder;
        // TODO: Show edit work order dialog
    }

    [RelayCommand]
    private async Task UpdateStatusAsync(WorkOrder workOrder)
    {
        if (workOrder == null) return;
        
        try
        {
            // Cycle through statuses
            var newStatus = workOrder.WorkStatus switch
            {
                WorkStatus.Received => WorkStatus.InProgress,
                WorkStatus.InProgress => WorkStatus.Ready,
                WorkStatus.Ready => WorkStatus.Delivered,
                WorkStatus.Delivered => WorkStatus.Completed,
                WorkStatus.Completed => WorkStatus.Completed,
                _ => WorkStatus.Received
            };
            
            await _workOrderService.UpdateStatusAsync(workOrder.Id, newStatus);
            workOrder.WorkStatus = newStatus;
            
            ShowSuccess($"Status updated to {newStatus}");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to update status: {ex.Message}");
        }
    }

    [RelayCommand]
    private void GenerateInvoice(WorkOrder workOrder)
    {
        SelectedWorkOrder = workOrder;
        _navigationService.NavigateTo("Invoice");
    }

    [RelayCommand]
    private async Task DeleteWorkOrderAsync(WorkOrder workOrder)
    {
        if (workOrder == null) return;

        try
        {
            await _workOrderService.DeleteAsync(workOrder.Id);
            WorkOrders.Remove(workOrder);
            ShowSuccess("Work order deleted successfully");
        }
        catch (Exception ex)
        {
            ShowError($"Cannot delete work order: {ex.Message}");
        }
    }
}
