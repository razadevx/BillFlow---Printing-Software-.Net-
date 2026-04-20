using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace BillFlow.ViewModels;

public partial class CustomersViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ICustomerService _customerService;
    private CancellationTokenSource? _searchDebounceCts;
    /// <summary>EF DbContext is not thread-safe; overlapping loads from ctor + search binding caused crashes.</summary>
    private readonly SemaphoreSlim _loadLock = new(1, 1);

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new();

    public ICollectionView CustomersView => CollectionViewSource.GetDefaultView(Customers);

    [ObservableProperty]
    private Customer? _selectedCustomer;

    [ObservableProperty]
    private int _totalCustomers;

    [ObservableProperty]
    private int _customersWithCredit;

    public bool HasCustomers => Customers?.Count > 0;
    public bool HasNoCustomers => Customers?.Count == 0;

    partial void OnCustomersChanged(ObservableCollection<Customer> value)
    {
        OnPropertyChanged(nameof(HasCustomers));
        OnPropertyChanged(nameof(HasNoCustomers));
        OnPropertyChanged(nameof(CustomersView));
    }

    partial void OnSearchQueryChanged(string value)
    {
        _ = DebouncedSearchAsync();
    }

    public CustomersViewModel(INavigationService navigationService, ICustomerService customerService)
    {
        _navigationService = navigationService;
        _customerService = customerService;
        PageTitle = "Customers";
        
        _ = LoadCustomersAsync();
    }

    [RelayCommand]
    public async Task LoadCustomersAsync()
    {
        await _loadLock.WaitAsync();
        try
        {
            IsLoading = true;
            ClearMessages();

            try
            {
                var customers = await _customerService.GetAllAsync();
                Customers = new ObservableCollection<Customer>(customers);
                TotalCustomers = customers.Count;
                CustomersWithCredit = customers.Count(c => c.TotalCredit > 0);
            }
            catch (Exception ex)
            {
                ShowError($"Failed to load customers: {ex.Message}");
            }
        }
        finally
        {
            IsLoading = false;
            _loadLock.Release();
        }
    }

    [RelayCommand]
    private async Task SearchCustomersAsync()
    {
        await _loadLock.WaitAsync();
        try
        {
            IsLoading = true;
            try
            {
                if (string.IsNullOrWhiteSpace(SearchQuery))
                {
                    var customers = await _customerService.GetAllAsync();
                    Customers = new ObservableCollection<Customer>(customers);
                    TotalCustomers = customers.Count;
                    CustomersWithCredit = customers.Count(c => c.TotalCredit > 0);
                }
                else
                {
                    var customers = await _customerService.SearchAsync(SearchQuery);
                    Customers = new ObservableCollection<Customer>(customers);
                    TotalCustomers = customers.Count;
                    CustomersWithCredit = customers.Count(c => c.TotalCredit > 0);
                }
            }
            catch (Exception ex)
            {
                ShowError($"Search failed: {ex.Message}");
            }
        }
        finally
        {
            IsLoading = false;
            _loadLock.Release();
        }
    }

    [ObservableProperty]
    private bool _showAddCustomerDialog;

    [ObservableProperty]
    private Customer? _editingCustomer;

    [RelayCommand]
    private void AddCustomer()
    {
        EditingCustomer = null;
        ShowAddCustomerDialog = true;
    }

    [RelayCommand]
    private void EditCustomer(Customer customer)
    {
        if (customer == null) return;
        SelectedCustomer = customer;
        EditingCustomer = customer;
        ShowAddCustomerDialog = true;
    }

    [RelayCommand]
    private void ViewCustomerDetails(Customer? customer)
    {
        if (customer == null) return;
        _navigationService.NavigateToCustomerDetail(customer.Id);
    }

    [RelayCommand]
    private async Task DeleteCustomer(Customer customer)
    {
        if (customer == null) return;

        try
        {
            await _customerService.DeleteAsync(customer.Id);
            Customers.Remove(customer);
            ShowSuccess("Customer deleted successfully");
        }
        catch (Exception ex)
        {
            ShowError($"Cannot delete customer: {ex.Message}");
        }
    }

    private async Task DebouncedSearchAsync()
    {
        _searchDebounceCts?.Cancel();
        _searchDebounceCts = new CancellationTokenSource();
        var token = _searchDebounceCts.Token;

        try
        {
            await Task.Delay(300, token);
            if (!token.IsCancellationRequested)
                await SearchCustomersAsync();
        }
        catch (TaskCanceledException)
        {
            // Expected during fast typing.
        }
    }
}
