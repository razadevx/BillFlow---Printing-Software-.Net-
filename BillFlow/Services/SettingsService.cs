using Microsoft.EntityFrameworkCore;
using BillFlow.Database;
using BillFlow.Models;

namespace BillFlow.Services;

public interface ISettingsService
{
    Task<AppSettings> GetSettingsAsync();
    Task UpdateSettingsAsync(AppSettings settings);
    Task<decimal> GetRatePerSqFtAsync();
    Task<string> GetBusinessNameAsync();
}

public class SettingsService : ISettingsService
{
    private readonly BillFlowDbContext _context;

    public SettingsService(BillFlowDbContext context)
    {
        _context = context;
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        AppSettings settings;

        try
        {
            settings = await _context.Settings
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
        catch (InvalidOperationException)
        {
            // Database has NULL values, create new settings
            settings = null;
        }

        if (settings == null)
        {
            settings = new AppSettings();
            _context.Settings.Add(settings);
            await _context.SaveChangesAsync();
        }

        return settings;
    }

    public async Task UpdateSettingsAsync(AppSettings settings)
    {
        settings.UpdatedAt = DateTime.Now;
        _context.Settings.Update(settings);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetRatePerSqFtAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.RatePerSqFt;
    }

    public async Task<string> GetBusinessNameAsync()
    {
        var settings = await GetSettingsAsync();
        return settings.BusinessName ?? "My Printing Business";
    }
}
