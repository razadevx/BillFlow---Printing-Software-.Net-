using System.Windows.Controls;
using BillFlow.Services;
using BillFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views;

public partial class CustomerDetailPage : UserControl
{
    public CustomerDetailPage()
    {
        InitializeComponent();
    }

    public CustomerDetailPage(CustomerDetailViewModel viewModel, INavigationService navigationService) : this()
    {
        DataContext = viewModel;
        Loaded += async (_, _) =>
        {
            if (navigationService.TryConsumeCustomerDetailId() is int id)
                await viewModel.LoadAsync(id);
        };
    }
}
