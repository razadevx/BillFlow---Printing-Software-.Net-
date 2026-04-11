using System.Windows;
using System.Windows.Controls;
using BillFlow.ViewModels;
using BillFlow.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views;

public partial class CustomersPage : UserControl
{
    public CustomersPage()
    {
        InitializeComponent();
        var viewModel = ((App)App.Current)._host?.Services.GetRequiredService<CustomersViewModel>();
        DataContext = viewModel;

        // Subscribe to show dialog requests
        if (viewModel != null)
        {
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.ShowAddCustomerDialog) && viewModel.ShowAddCustomerDialog)
                {
                    ShowAddCustomerDialog();
                    viewModel.ShowAddCustomerDialog = false;
                }
            };
        }
    }

    private void ShowAddCustomerDialog()
    {
        var dialog = ((App)App.Current)._host?.Services.GetRequiredService<AddCustomerDialog>();
        if (dialog != null)
        {
            dialog.Owner = Window.GetWindow(this);
            var result = dialog.ShowDialog();
            if (result == true)
            {
                // Refresh the list if customer was added
                if (DataContext is CustomersViewModel vm)
                {
                    _ = vm.LoadCustomersAsync();
                }
            }
        }
    }
}
