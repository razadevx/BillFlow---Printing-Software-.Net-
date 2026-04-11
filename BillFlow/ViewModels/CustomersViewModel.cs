using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;
using System.Collections.ObjectModel;

namespace BillFlow.ViewModels;

public partial class CustomersViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;
    private readonly ICustomerService _customerService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new();

    [ObservableProperty]
    private Customer? _selectedCustomer;

    [ObservableProperty]
    private int _totalCustomers;

    [ObservableProperty]
    private int _customersWithCredit;

    partial void OnSearchQueryChanged(string value)
    {
        _ = SearchCustomersAsync();
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
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SearchCustomersAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            await LoadCustomersAsync();
            return;
        }

        IsLoading = true;
        try
        {
            var customers = await _customerService.SearchAsync(SearchQuery);
            Customers = new ObservableCollection<Customer>(customers);
        }
        catch (Exception ex)
        {
            ShowError($"Search failed: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [ObservableProperty]
    private bool _showAddCustomerDialog;

    [RelayCommand]
    private void AddCustomer()
    {
        ShowAddCustomerDialog = true;
    }

    [RelayCommand]
    private void EditCustomer(Customer customer)
    {
        SelectedCustomer = customer;
        // TODO: Show edit customer dialog
    }

    [RelayCommand]
    private void ViewCustomerDetails(Customer? customer)
    {
        if (customer == null) return;
        _navigationService.NavigateToCustomerDetail(customer.Id);
    }

    [RelayCommand]
    private async Task DeleteCustomerAsync(Customer customer)
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
}
