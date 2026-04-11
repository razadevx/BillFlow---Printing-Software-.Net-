using System.Windows.Controls;
using BillFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views;

public partial class DailySchedulePage : UserControl
{
    public DailySchedulePage()
    {
        InitializeComponent();
        DataContext = ((App)App.Current)._host?.Services.GetRequiredService<DailyScheduleViewModel>();
    }
}
