using System.Windows;
using System.Windows.Controls;
using BillFlow.ViewModels;
using BillFlow.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace BillFlow.Views;

public partial class KhataLedgerPage : UserControl
{
    public KhataLedgerPage()
    {
        InitializeComponent();
        var viewModel = ((App)App.Current)._host?.Services.GetRequiredService<KhataLedgerViewModel>();
        DataContext = viewModel;

        if (viewModel != null)
        {
            viewModel.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.ShowRecordPaymentDialog) && viewModel.ShowRecordPaymentDialog)
                {
                    ShowRecordPaymentDialog();
                    viewModel.ShowRecordPaymentDialog = false;
                    await viewModel.LoadKhataSummaryAsync();
                }
            };
        }
    }

    private void ShowRecordPaymentDialog()
    {
        if (DataContext is not KhataLedgerViewModel vm || vm.SelectedSummary?.Customer == null)
            return;

        var dialog = ((App)App.Current)._host?.Services.GetRequiredService<RecordPaymentDialog>();
        if (dialog?.DataContext is ViewModels.Dialogs.RecordPaymentDialogViewModel dialogVm)
        {
            dialogVm.SetCustomer(vm.SelectedSummary.Customer);
        }

        if (dialog != null)
        {
            dialog.Owner = Window.GetWindow(this);
            var result = dialog.ShowDialog();
            if (result == true && vm.SelectedSummary != null)
            {
                _ = vm.ViewCustomerKhataCommand.ExecuteAsync(vm.SelectedSummary);
            }
        }
    }
}
