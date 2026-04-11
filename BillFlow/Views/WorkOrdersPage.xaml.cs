using System.Windows.Controls;
using BillFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views;

public partial class WorkOrdersPage : UserControl
{
    public WorkOrdersPage()
    {
        InitializeComponent();
        DataContext = ((App)App.Current)._host?.Services.GetRequiredService<WorkOrdersViewModel>();
    }
}
