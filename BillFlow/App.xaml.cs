using System.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BillFlow.Database;
using BillFlow.Services;
using BillFlow.ViewModels;
using BillFlow.ViewModels.Dialogs;
using BillFlow.Views;
using BillFlow.Views.Dialogs;

namespace BillFlow;

public partial class App : Application
{
    internal IHost? _host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Database
                services.AddDbContext<BillFlowDbContext>(options =>
                {
                    var dbPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "BillFlow",
                        "billflow.db"
                    );
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath)!);
                    options.UseSqlite($"Data Source={dbPath}");
                });

                // Services
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ICalculationService, CalculationService>();
                services.AddScoped<ICustomerService, CustomerService>();
                services.AddScoped<IWorkOrderService, WorkOrderService>();
                services.AddScoped<IKhataService, KhataService>();
                services.AddScoped<IInvoiceService, InvoiceService>();
                services.AddScoped<ISettingsService, SettingsService>();

                // ViewModels
                services.AddSingleton<MainViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<CustomersViewModel>();
                services.AddTransient<WorkOrdersViewModel>();
                services.AddTransient<DailyScheduleViewModel>();
                services.AddTransient<KhataLedgerViewModel>();
                services.AddTransient<InvoiceViewModel>();
                services.AddTransient<SettingsViewModel>();
                services.AddTransient<AddCustomerDialogViewModel>();
                services.AddTransient<RecordPaymentDialogViewModel>();
                services.AddTransient<WorkOrderCreateViewModel>();
                services.AddTransient<CustomerDetailViewModel>();

                // Views (for DI navigation)
                services.AddTransient<DashboardPage>();
                services.AddTransient<CustomersPage>();
                services.AddTransient<WorkOrdersPage>();
                services.AddTransient<WorkOrderCreatePage>();
                services.AddTransient<DailySchedulePage>();
                services.AddTransient<KhataLedgerPage>();
                services.AddTransient<InvoicePage>();
                services.AddTransient<SettingsPage>();
                services.AddTransient<AddCustomerDialog>();
                services.AddTransient<RecordPaymentDialog>();
                services.AddTransient<CustomerDetailPage>();

                // Main Window
                services.AddSingleton<MainWindow>();
            })
            .Build();

        var dbPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BillFlow",
            "billflow.db");

        if (System.IO.File.Exists(dbPath))
        {
            var deleteLegacy = false;
            await using (var conn = new SqliteConnection($"Data Source={dbPath}"))
            {
                await conn.OpenAsync();
                await using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory' LIMIT 1";
                var hasHistoryTable = await cmd.ExecuteScalarAsync() is not null;
                if (!hasHistoryTable)
                {
                    deleteLegacy = true;
                }
                else
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM __EFMigrationsHistory";
                    var appliedCount = Convert.ToInt64(await cmd.ExecuteScalarAsync());
                    cmd.CommandText = "SELECT 1 FROM sqlite_master WHERE type='table' AND name='Settings' LIMIT 1";
                    var hasSettingsTable = await cmd.ExecuteScalarAsync() is not null;
                    if (appliedCount == 0 && hasSettingsTable)
                        deleteLegacy = true;
                }
            }

            if (deleteLegacy)
            {
                SqliteConnection.ClearAllPools();
                System.IO.File.Delete(dbPath);
            }
        }

        try
        {
            using (var scope = _host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BillFlowDbContext>();
                await dbContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)
            || ex.InnerException?.Message?.Contains("already exists", StringComparison.OrdinalIgnoreCase) == true)
        {
            SqliteConnection.ClearAllPools();
            if (System.IO.File.Exists(dbPath))
                System.IO.File.Delete(dbPath);
            using (var scope = _host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BillFlowDbContext>();
                await dbContext.Database.MigrateAsync();
            }
        }

        // Show Main Window
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(e);
    }
}
