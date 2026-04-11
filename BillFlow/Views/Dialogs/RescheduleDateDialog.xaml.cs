using System.Windows;

namespace BillFlow.Views.Dialogs;

public partial class RescheduleDateDialog : Window
{
    public DateTime SelectedDate { get; private set; }

    public RescheduleDateDialog(DateTime currentDate)
    {
        InitializeComponent();
        DatePickerControl.SelectedDate = currentDate.Date;
        Owner = Application.Current.MainWindow;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        SelectedDate = DatePickerControl.SelectedDate?.Date ?? DateTime.Today;
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
