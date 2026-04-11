using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using System.Collections.ObjectModel;

namespace BillFlow.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    /// <summary>Route key from <see cref="INavigationService"/> — used for sidebar selection (must match nav item names).</summary>
    [ObservableProperty]
    private string _currentRoute = "Dashboard";

    /// <summary>Shown in the main header — human-readable title.</summary>
    [ObservableProperty]
    private string _pageHeader = "Dashboard";

    [ObservableProperty]
    private bool _isSidebarExpanded = true;

    public ObservableCollection<NavItem> NavigationItems { get; } = new();

    public MainViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        _navigationService.Navigated += OnNavigated;

        InitializeNavigation();
    }

    private void InitializeNavigation()
    {
        NavigationItems.Add(new NavItem { Name = "Dashboard", Title = "Dashboard", IconGlyph = "🏠" });
        NavigationItems.Add(new NavItem { Name = "Customers", Title = "Customers", IconGlyph = "👥" });
        NavigationItems.Add(new NavItem { Name = "WorkOrders", Title = "Work Orders", IconGlyph = "📋" });
        NavigationItems.Add(new NavItem { Name = "DailySchedule", Title = "Daily Schedule", IconGlyph = "📅" });
        NavigationItems.Add(new NavItem { Name = "KhataLedger", Title = "Khata Ledger", IconGlyph = "📖" });
        NavigationItems.Add(new NavItem { Name = "Invoice", Title = "Invoice", IconGlyph = "📄" });
        NavigationItems.Add(new NavItem { Name = "Settings", Title = "Settings", IconGlyph = "⚙️", IsFooter = true });
    }

    private void OnNavigated(object? sender, string pageName)
    {
        CurrentRoute = pageName;
        PageHeader = pageName switch
        {
            "CustomerDetail" => "Customer",
            "WorkOrderCreate" => "New Work Order",
            "KhataLedger" => "Khata Ledger",
            "DailySchedule" => "Daily Schedule",
            "WorkOrders" => "Work Orders",
            _ => pageName
        };
    }

    [RelayCommand]
    private void Navigate(string pageName)
    {
        _navigationService.NavigateTo(pageName);
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarExpanded = !IsSidebarExpanded;
    }
}

public class NavItem
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string IconGlyph { get; set; } = "•";
    public bool IsFooter { get; set; }
}
