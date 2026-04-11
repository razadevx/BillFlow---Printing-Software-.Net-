using System.Windows;
using BillFlow.ViewModels.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views.Dialogs;

public partial class RecordPaymentDialog : Window
{
    public RecordPaymentDialog()
    {
        InitializeComponent();
        DataContext = ((App)App.Current)._host?.Services.GetRequiredService<RecordPaymentDialogViewModel>();
        
        // Close dialog when RequestClose event is raised
        if (DataContext is RecordPaymentDialogViewModel vm)
        {
            vm.RequestClose += (s, result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
