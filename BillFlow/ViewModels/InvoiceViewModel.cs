using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;
using System.IO;

namespace BillFlow.ViewModels;

public partial class InvoiceViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IInvoiceService _invoiceService;
    private readonly ICustomerService _customerService;
    private readonly IWorkOrderService _workOrderService;

    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new();

    [ObservableProperty]
    private Customer? _selectedCustomer;

    [ObservableProperty]
    private ObservableCollection<WorkOrder> _pendingWorkOrders = new();

    [ObservableProperty]
    private WorkOrder? _selectedWorkOrder;

    [ObservableProperty]
    private string _invoiceNumber = "INV-2026-0001";

    [ObservableProperty]
    private DateTime _invoiceDate = DateTime.Today;

    partial void OnSelectedCustomerChanged(Customer? value)
    {
        if (value != null)
        {
            _ = LoadPendingWorkOrdersAsync(value.Id);
        }
    }

    partial void OnSelectedWorkOrderChanged(WorkOrder? value) =>
        GenerateInvoiceCommand.NotifyCanExecuteChanged();

    public InvoiceViewModel(
        INavigationService navigationService,
        IInvoiceService invoiceService,
        ICustomerService customerService,
        IWorkOrderService workOrderService)
    {
        _navigationService = navigationService;
        _invoiceService = invoiceService;
        _customerService = customerService;
        _workOrderService = workOrderService;
        PageTitle = "Invoice";
        
        _ = LoadCustomersAsync();
    }

    [RelayCommand]
    private async Task LoadCustomersAsync()
    {
        IsLoading = true;
        ClearMessages();

        try
        {
            var customers = await _customerService.GetAllAsync();
            Customers.Clear();
            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load customers: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task LoadPendingWorkOrdersAsync(int customerId)
    {
        try
        {
            var orders = await _workOrderService.GetByCustomerAsync(customerId);
            // Filter pending orders (not fully paid)
            var pending = orders.Where(o => o.PendingAmount > 0).ToList();
            PendingWorkOrders.Clear();
            foreach (var order in pending)
            {
                PendingWorkOrders.Add(order);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load work orders: {ex.Message}");
        }
    }

    [RelayCommand(CanExecute = nameof(CanGenerateInvoice))]
    private async Task GenerateInvoiceAsync()
    {
        if (SelectedWorkOrder == null)
        {
            ShowError("Please select a work order");
            return;
        }

        IsLoading = true;
        ClearMessages();

        try
        {
            var pdfBytes = await _invoiceService.GenerateInvoicePdfAsync(SelectedWorkOrder.Id);
            
            // Save to file
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Invoice_{SelectedWorkOrder.OrderCode}",
                DefaultExt = ".pdf",
                Filter = "PDF documents (.pdf)|*.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                await File.WriteAllBytesAsync(saveDialog.FileName, pdfBytes);
                ShowSuccess($"Invoice saved to {saveDialog.FileName}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to generate invoice: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GenerateStatementAsync()
    {
        if (SelectedCustomer == null)
        {
            ShowError("Please select a customer");
            return;
        }

        IsLoading = true;
        ClearMessages();

        try
        {
            var pdfBytes = await _invoiceService.GenerateCustomerStatementPdfAsync(SelectedCustomer.Id);
            
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"Statement_{SelectedCustomer.CustomerCode}",
                DefaultExt = ".pdf",
                Filter = "PDF documents (.pdf)|*.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                await File.WriteAllBytesAsync(saveDialog.FileName, pdfBytes);
                ShowSuccess($"Statement saved to {saveDialog.FileName}");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Failed to generate statement: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    public bool CanGenerateInvoice => SelectedWorkOrder != null;
}
