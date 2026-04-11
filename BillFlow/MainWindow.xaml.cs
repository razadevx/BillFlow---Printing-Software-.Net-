using System.Windows;
using System.Windows.Controls;
using BillFlow.Services;
using BillFlow.ViewModels;

namespace BillFlow;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel, INavigationService navigationService)
    {
        InitializeComponent();
        ViewModel = viewModel;
        DataContext = viewModel;

        navigationService.SetFrame(MainFrame);
        navigationService.NavigateTo("Dashboard");
    }

    public MainViewModel ViewModel { get; }
}
