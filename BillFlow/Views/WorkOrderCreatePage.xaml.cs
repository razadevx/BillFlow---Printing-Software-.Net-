using System.Windows.Controls;
using BillFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views;

public partial class WorkOrderCreatePage : UserControl
{
    public WorkOrderCreatePage()
    {
        InitializeComponent();
        DataContext = ((App)App.Current)._host?.Services.GetRequiredService<WorkOrderCreateViewModel>();
    }
}
