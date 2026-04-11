using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BillFlow.ViewModels;

public abstract partial class ViewModelBase : ObservableObject
{
    [ObservableProperty]
    private string _pageTitle = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _errorMessage;

    [ObservableProperty]
    private string? _successMessage;

    protected void ShowError(string message)
    {
        ErrorMessage = message;
        SuccessMessage = null;
    }

    protected void ShowSuccess(string message)
    {
        SuccessMessage = message;
        ErrorMessage = null;
    }

    protected void ClearMessages()
    {
        ErrorMessage = null;
        SuccessMessage = null;
    }
}
