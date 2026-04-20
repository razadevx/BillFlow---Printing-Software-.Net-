using System.Diagnostics;
using System.Windows.Controls;
using BillFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views;

public partial class WorkOrdersPage : UserControl
{
    public WorkOrdersPage()
    {
        Debug.WriteLine("[DEBUG] WorkOrdersPage - Constructor START");
        InitializeComponent();
        Debug.WriteLine("[DEBUG] WorkOrdersPage - InitializeComponent done");

        var host = ((App)App.Current)._host;
        Debug.WriteLine($"[DEBUG] WorkOrdersPage - Host is null: {host == null}");

        if (host != null)
        {
            var viewModel = host.Services.GetRequiredService<WorkOrdersViewModel>();
            Debug.WriteLine($"[DEBUG] WorkOrdersPage - ViewModel retrieved: {viewModel != null}");
            DataContext = viewModel;
        }
        else
        {
            Debug.WriteLine("[DEBUG ERROR] WorkOrdersPage - Host is NULL!");
        }

        Debug.WriteLine("[DEBUG] WorkOrdersPage - Constructor END");
    }
}
