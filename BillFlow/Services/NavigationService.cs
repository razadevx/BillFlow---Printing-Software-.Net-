using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using BillFlow.Views;

namespace BillFlow.Services;

public interface INavigationService
{
    void NavigateTo(string pageName);
    void NavigateTo<T>() where T : UserControl;
    void SetFrame(Frame frame);
    void NavigateToCustomerDetail(int customerId);
    int? TryConsumeCustomerDetailId();
    void NavigateToWorkOrderEdit(int workOrderId);
    int? TryConsumeWorkOrderEditId();
    string? CurrentPage { get; }
    event EventHandler<string>? Navigated;
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private Frame? _frame;
    private int? _pendingCustomerDetailId;
    private int? _pendingWorkOrderEditId;

    public string? CurrentPage { get; private set; }
    public event EventHandler<string>? Navigated;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void SetFrame(Frame frame)
    {
        _frame = frame;
    }

    public void NavigateTo(string pageName)
    {
        UserControl? page = pageName switch
        {
            "Dashboard" => _serviceProvider.GetRequiredService<DashboardPage>(),
            "Customers" => _serviceProvider.GetRequiredService<CustomersPage>(),
            "WorkOrders" => _serviceProvider.GetRequiredService<WorkOrdersPage>(),
            "WorkOrderCreate" => _serviceProvider.GetRequiredService<WorkOrderCreatePage>(),
            "DailySchedule" => _serviceProvider.GetRequiredService<DailySchedulePage>(),
            "KhataLedger" => _serviceProvider.GetRequiredService<KhataLedgerPage>(),
            "Invoice" => _serviceProvider.GetRequiredService<InvoicePage>(),
            "Settings" => _serviceProvider.GetRequiredService<SettingsPage>(),
            "CustomerDetail" => _serviceProvider.GetRequiredService<CustomerDetailPage>(),
            _ => null
        };

        if (page != null && _frame != null)
        {
            _frame.Navigate(page);
            CurrentPage = pageName;
            Navigated?.Invoke(this, pageName);
        }
    }

    public void NavigateTo<T>() where T : UserControl
    {
        var page = _serviceProvider.GetRequiredService<T>();
        if (_frame != null)
        {
            _frame.Navigate(page);
            CurrentPage = typeof(T).Name;
            Navigated?.Invoke(this, CurrentPage);
        }
    }

    public void NavigateToCustomerDetail(int customerId)
    {
        _pendingCustomerDetailId = customerId;
        NavigateTo("CustomerDetail");
    }

    public int? TryConsumeCustomerDetailId()
    {
        var id = _pendingCustomerDetailId;
        _pendingCustomerDetailId = null;
        return id;
    }

    public void NavigateToWorkOrderEdit(int workOrderId)
    {
        _pendingWorkOrderEditId = workOrderId;
        NavigateTo("WorkOrderCreate");
    }

    public int? TryConsumeWorkOrderEditId()
    {
        var id = _pendingWorkOrderEditId;
        _pendingWorkOrderEditId = null;
        return id;
    }
}
