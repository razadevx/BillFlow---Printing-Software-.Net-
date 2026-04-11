using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BillFlow.Services;
using BillFlow.Models;

namespace BillFlow.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private string _businessName = "My Printing Business";

    [ObservableProperty]
    private string _ownerName = "";

    [ObservableProperty]
    private string _phone = "";

    [ObservableProperty]
    private string _address = "";

    [ObservableProperty]
    private decimal _ratePerSqFt = 50m;

    [ObservableProperty]
    private decimal _defaultCreditLimit = 10000m;

    [ObservableProperty]
    private string _invoicePrefix = "INV";

    [ObservableProperty]
    private string _invoiceTerms = "Payment due within 15 days. Thank you for your business!";

    [ObservableProperty]
    private string _currencySymbol = "PKR";

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
        PageTitle = "Settings";
        
        _ = LoadSettingsAsync();
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        IsLoading = true;
        ClearMessages();
        
        try
        {
            var settings = await _settingsService.GetSettingsAsync();
            
            settings.BusinessName = BusinessName;
            settings.OwnerName = OwnerName;
            settings.Phone = Phone;
            settings.Address = Address;
            settings.RatePerSqFt = RatePerSqFt;
            settings.DefaultCreditLimit = DefaultCreditLimit;
            settings.InvoicePrefix = InvoicePrefix;
            
            await _settingsService.UpdateSettingsAsync(settings);
            
            ShowSuccess("Settings saved successfully!");
        }
        catch (Exception ex)
        {
            ShowError($"Failed to save settings: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task LoadSettingsAsync()
    {
        IsLoading = true;
        ClearMessages();
        
        try
        {
            var settings = await _settingsService.GetSettingsAsync();
            
            BusinessName = settings.BusinessName;
            OwnerName = settings.OwnerName;
            Phone = settings.Phone;
            Address = settings.Address;
            RatePerSqFt = settings.RatePerSqFt;
            DefaultCreditLimit = settings.DefaultCreditLimit;
            InvoicePrefix = settings.InvoicePrefix;
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load settings: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
