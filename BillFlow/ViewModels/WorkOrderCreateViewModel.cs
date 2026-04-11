using System.Collections.Specialized;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;

namespace BillFlow.ViewModels;

public partial class WorkOrderCreateViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly IWorkOrderService _workOrderService;
    private readonly ICustomerService _customerService;
    private readonly ICalculationService _calculationService;

    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new();

    [ObservableProperty]
    private Customer? _selectedCustomer;

    [ObservableProperty]
    private DateTime _scheduledDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<LineItemViewModel> _lineItems = new();

    [ObservableProperty]
    private decimal _pastBill = 0;

    [ObservableProperty]
    private decimal _amountPaid = 0;

    [ObservableProperty]
    private PaymentStatus _selectedPaymentStatus = PaymentStatus.Credit;

    [ObservableProperty]
    private string _notes = string.Empty;

    // Computed properties
    public decimal TotalArea => LineItems.Sum(i => i.Area);
    public decimal CurrentBill => LineItems.Sum(i => i.Total);
    public decimal GrandTotal => CurrentBill + PastBill;
    public decimal PendingAmount => GrandTotal - AmountPaid;

    public List<PaymentStatus> PaymentStatuses => Enum.GetValues<PaymentStatus>().ToList();

    partial void OnAmountPaidChanged(decimal value)
    {
        OnPropertyChanged(nameof(PendingAmount));
    }

    public WorkOrderCreateViewModel(
        INavigationService navigationService,
        IWorkOrderService workOrderService,
        ICustomerService customerService,
        ICalculationService calculationService)
    {
        _navigationService = navigationService;
        _workOrderService = workOrderService;
        _customerService = customerService;
        _calculationService = calculationService;
        PageTitle = "Create Work Order";

        LineItems.CollectionChanged += OnLineItemsCollectionChanged;

        // Add one empty line item by default
        AddLineItem();

        // Load customers
        _ = LoadCustomersAsync();
    }

    private void OnLineItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (LineItemViewModel item in e.NewItems)
                item.PropertyChanged += (_, args) =>
                {
                    if (args.PropertyName is nameof(LineItemViewModel.Width) or nameof(LineItemViewModel.Height)
                        or nameof(LineItemViewModel.Quantity) or nameof(LineItemViewModel.Rate))
                        CreateWorkOrderCommand.NotifyCanExecuteChanged();
                };
        }

        CreateWorkOrderCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedCustomerChanged(Customer? value) =>
        CreateWorkOrderCommand.NotifyCanExecuteChanged();

    private async Task LoadCustomersAsync()
    {
        try
        {
            var customers = await _customerService.GetAllAsync();
            Customers = new ObservableCollection<Customer>(customers);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load customers: {ex.Message}");
        }
    }

    [RelayCommand]
    private void AddLineItem()
    {
        var newItem = new LineItemViewModel(_calculationService);
        newItem.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(LineItemViewModel.Total) || 
                e.PropertyName == nameof(LineItemViewModel.Area))
            {
                OnPropertyChanged(nameof(TotalArea));
                OnPropertyChanged(nameof(CurrentBill));
                OnPropertyChanged(nameof(GrandTotal));
                OnPropertyChanged(nameof(PendingAmount));
            }
        };
        LineItems.Add(newItem);
    }

    [RelayCommand]
    private void RemoveLineItem(LineItemViewModel item)
    {
        if (LineItems.Count > 1)
        {
            LineItems.Remove(item);
            OnPropertyChanged(nameof(TotalArea));
            OnPropertyChanged(nameof(CurrentBill));
            OnPropertyChanged(nameof(GrandTotal));
            OnPropertyChanged(nameof(PendingAmount));
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private async Task CreateWorkOrderAsync()
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
            var order = new WorkOrder
            {
                CustomerId = SelectedCustomer.Id,
                ScheduledDate = ScheduledDate,
                PastBill = PastBill,
                AmountPaid = AmountPaid,
                PaymentStatus = SelectedPaymentStatus,
                Notes = Notes
            };

            var lineItems = LineItems.Select(vm => new LineItem
            {
                Description = vm.Description,
                Width = vm.Width,
                Height = vm.Height,
                Quantity = vm.Quantity,
                Rate = vm.Rate
            }).ToList();

            await _workOrderService.CreateAsync(order, lineItems);

            ShowSuccess("Work order created successfully");
            
            // Navigate back or clear form
            _navigationService.NavigateTo("WorkOrders");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to create work order: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        _navigationService.NavigateTo("WorkOrders");
    }

    public bool CanCreate => SelectedCustomer != null && LineItems.Any(i => i.Width > 0 && i.Height > 0);
}

public partial class LineItemViewModel : ObservableObject
{
    private readonly ICalculationService _calculationService;

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private decimal _width = 0;

    [ObservableProperty]
    private decimal _height = 0;

    [ObservableProperty]
    private int _quantity = 1;

    [ObservableProperty]
    private decimal _rate = 50m; // Default rate

    public decimal Area
    {
        get
        {
            var li = new LineItem { Width = Width, Height = Height, Quantity = Quantity, Rate = Rate };
            _calculationService.RecalculateLineItem(li);
            return li.Area;
        }
    }

    public decimal Total
    {
        get
        {
            var li = new LineItem { Width = Width, Height = Height, Quantity = Quantity, Rate = Rate };
            _calculationService.RecalculateLineItem(li);
            return li.Total;
        }
    }

    public LineItemViewModel(ICalculationService calculationService)
    {
        _calculationService = calculationService;
    }

    partial void OnWidthChanged(decimal value) => NotifyCalculatedProperties();
    partial void OnHeightChanged(decimal value) => NotifyCalculatedProperties();
    partial void OnQuantityChanged(int value) => NotifyCalculatedProperties();
    partial void OnRateChanged(decimal value) => NotifyCalculatedProperties();

    private void NotifyCalculatedProperties()
    {
        OnPropertyChanged(nameof(Area));
        OnPropertyChanged(nameof(Total));
    }
}
