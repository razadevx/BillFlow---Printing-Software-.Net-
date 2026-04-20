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
                }, ServiceLifetime.Transient);

                // Services
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ICalculationService, CalculationService>();
                services.AddTransient<ICustomerService, CustomerService>();
                services.AddTransient<IWorkOrderService, WorkOrderService>();
                services.AddTransient<IKhataService, KhataService>();
                services.AddTransient<IInvoiceService, InvoiceService>();
                services.AddTransient<ISettingsService, SettingsService>();

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

        try
        {
            using (var scope = _host.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BillFlowDbContext>();
                await dbContext.Database.MigrateAsync();
                await EnsureCustomerCreditRiskColumnAsync(dbContext);
                await EnsureSettingsColumnsAsync(dbContext);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Database migration failed: {ex.Message}{Environment.NewLine}{Environment.NewLine}Existing data was left unchanged.",
                "BillFlow Startup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
            return;
        }

        // Show Main Window
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static async Task EnsureCustomerCreditRiskColumnAsync(BillFlowDbContext dbContext)
    {
        var connection = (SqliteConnection)dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "PRAGMA table_info('Customers');";

        var hasCreditRisk = false;
        await using (var reader = await checkCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                if (string.Equals(reader["name"]?.ToString(), "CreditRisk", StringComparison.OrdinalIgnoreCase))
                {
                    hasCreditRisk = true;
                    break;
                }
            }
        }

        if (!hasCreditRisk)
        {
            await using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = "ALTER TABLE Customers ADD COLUMN CreditRisk INTEGER NOT NULL DEFAULT 0;";
            await alterCommand.ExecuteNonQueryAsync();
        }
    }

    private static async Task EnsureSettingsColumnsAsync(BillFlowDbContext dbContext)
    {
        var connection = (SqliteConnection)dbContext.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
            await connection.OpenAsync();

        await using var checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "PRAGMA table_info('Settings');";

        var hasInvoiceTerms = false;
        var hasCurrencySymbol = false;

        await using (var reader = await checkCommand.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var colName = reader["name"]?.ToString();
                if (string.Equals(colName, "InvoiceTerms", StringComparison.OrdinalIgnoreCase)) hasInvoiceTerms = true;
                if (string.Equals(colName, "CurrencySymbol", StringComparison.OrdinalIgnoreCase)) hasCurrencySymbol = true;
            }
        }

        if (!hasInvoiceTerms)
        {
            await using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = "ALTER TABLE Settings ADD COLUMN InvoiceTerms TEXT NOT NULL DEFAULT 'Payment due within 15 days. Thank you for your business!';";
            await alterCommand.ExecuteNonQueryAsync();
        }

        if (!hasCurrencySymbol)
        {
            await using var alterCommand = connection.CreateCommand();
            alterCommand.CommandText = "ALTER TABLE Settings ADD COLUMN CurrencySymbol TEXT NOT NULL DEFAULT 'PKR';";
            await alterCommand.ExecuteNonQueryAsync();
        }
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
