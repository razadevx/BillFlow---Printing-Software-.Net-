using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

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

    [ObservableProperty]
    private WorkStatusFilterOption? _selectedStatusOption;

    public List<WorkStatusFilterOption> WorkStatusFilters { get; } = new()
    {
        new("All Status", null),
        new("Received", WorkStatus.Received),
        new("In Progress", WorkStatus.InProgress),
        new("Ready", WorkStatus.Ready),
        new("Delivered", WorkStatus.Delivered),
        new("Completed", WorkStatus.Completed)
    };

    partial void OnSelectedStatusFilterChanged(WorkStatus? value)
    {
        _ = FilterWorkOrdersAsync();
    }

    partial void OnSelectedStatusOptionChanged(WorkStatusFilterOption? value)
    {
        SelectedStatusFilter = value?.Status;
    }

    public WorkOrdersViewModel(INavigationService navigationService, IWorkOrderService workOrderService)
    {
        Debug.WriteLine("[DEBUG] WorkOrdersViewModel - Constructor START");
        _navigationService = navigationService;
        _workOrderService = workOrderService;
        PageTitle = "Work Orders";
        SelectedStatusOption = WorkStatusFilters.First();
        Debug.WriteLine("[DEBUG] WorkOrdersViewModel - Constructor END, starting async load...");

        // Use Task.Run to avoid blocking UI thread during construction
        Task.Run(async () =>
        {
            try
            {
                await LoadWorkOrdersAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DEBUG ERROR] WorkOrdersViewModel - Constructor async load failed: {ex}");
            }
        });
    }

    [RelayCommand]
    private async Task LoadWorkOrdersAsync()
    {
        Debug.WriteLine("[DEBUG] LoadWorkOrdersAsync - START");
        IsLoading = true;
        ClearMessages();

        try
        {
            Debug.WriteLine($"[DEBUG] LoadWorkOrdersAsync - Calling service... Filter: {SelectedStatusFilter}");
            List<WorkOrder>? orders = null;

            if (SelectedStatusFilter.HasValue)
            {
                orders = await _workOrderService.GetByStatusAsync(SelectedStatusFilter.Value);
            }
            else
            {
                orders = await _workOrderService.GetAllAsync();
            }

            Debug.WriteLine($"[DEBUG] LoadWorkOrdersAsync - Got {orders?.Count ?? 0} orders from service");

            if (orders == null)
            {
                Debug.WriteLine("[DEBUG ERROR] LoadWorkOrdersAsync - Service returned NULL!");
                ShowError("Failed to load work orders: Service returned null");
                return;
            }

            // Switch to UI thread for ObservableCollection operations
            await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Debug.WriteLine("[DEBUG] LoadWorkOrdersAsync - Clearing and repopulating collection on UI thread");
                // Clear and repopulate to avoid collection recreation (which causes WPF layout loop)
                WorkOrders.Clear();
                foreach (var order in orders)
                {
                    WorkOrders.Add(order);
                }
                Debug.WriteLine("[DEBUG] LoadWorkOrdersAsync - Collection updated");
            });

            // Update stats
            TotalWorkOrders = orders.Count;
            PendingWorkOrders = orders.Count(o => o.WorkStatus != WorkStatus.Completed && o.WorkStatus != WorkStatus.Delivered);
            CompletedToday = orders.Count(o => o.WorkStatus == WorkStatus.Completed && o.OrderDate.Date == DateTime.Today);
            Debug.WriteLine($"[DEBUG] LoadWorkOrdersAsync - Stats updated: Total={TotalWorkOrders}, Pending={PendingWorkOrders}, CompletedToday={CompletedToday}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[DEBUG ERROR] LoadWorkOrdersAsync - Exception: {ex}");
            ShowError($"Failed to load work orders: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
            Debug.WriteLine("[DEBUG] LoadWorkOrdersAsync - END");
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
        if (workOrder == null) return;
        SelectedWorkOrder = workOrder;
        _navigationService.NavigateToWorkOrderEdit(workOrder.Id);
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

public record WorkStatusFilterOption(string Label, WorkStatus? Status);
