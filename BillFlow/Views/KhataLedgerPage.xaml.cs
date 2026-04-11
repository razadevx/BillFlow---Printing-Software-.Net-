using System.Windows.Controls;
using BillFlow.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views;

public partial class KhataLedgerPage : UserControl
{
    public KhataLedgerPage()
    {
        InitializeComponent();
        DataContext = ((App)App.Current)._host?.Services.GetRequiredService<KhataLedgerViewModel>();
    }
}
