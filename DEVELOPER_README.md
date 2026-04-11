# BillFlow Developer Reference

**Complete technical reference for maintaining and extending BillFlow.**

---

## Project Structure

```
BillFlow/
├── App.xaml                    # Application startup, DI configuration
├── App.xaml.cs                 # Dependency Injection setup (11 services)
├── MainWindow.xaml             # Main shell, navigation
├── MainWindow.xaml.cs          # Code-behind
├── ViewModelLocator.cs         # ViewModel service locator
│
├── Models/
│   └── Models.cs               # 12 entity classes (Customer, WorkOrder, etc.)
│
├── Database/
│   └── BillFlowContext.cs      # EF Core DbContext, 12 DbSets
│
├── Services/                   # Business logic layer
│   ├── ValidationService.cs    # Business rules, ServiceResult pattern
│   ├── LoggingService.cs       # Centralized logging, file persistence
│   ├── SettingsService.cs      # Configuration management, 30-min cache
│   ├── CalculationService.cs   # SqFt, cost, tax calculations
│   ├── CreditAlertService.cs   # Credit monitoring, risk assessment
│   ├── OfflineSyncService.cs   # Offline queue, 5-min auto-sync
│   ├── BackupService.cs        # Backup/restore, daily auto-backups
│   ├── DiscountService.cs      # Discount mgmt, audit trail
│   ├── NotificationService.cs  # SMS/WhatsApp/Email, provider-agnostic
│   ├── ExportImportService.cs  # CSV/JSON export/import
│   ├── CustomerService.cs      # CRUD + validation/logging
│   ├── WorkOrderService.cs     # CRUD + validation/logging
│   ├── InvoiceService.cs       # PDF generation
│   ├── LedgerService.cs        # Ledger tracking + logging
│   └── DashboardService.cs     # Dashboard metrics
│
├── ViewModels/                 # UI binding layer
│   ├── MainViewModel.cs        # Navigation (8 destination VMs)
│   ├── DashboardViewModel.cs   # ENHANCED 2.0: With alerts, error handling
│   ├── CustomersViewModel.cs   # Customer CRUD
│   ├── WorkOrdersViewModel.cs  # Work order CRUD
│   ├── InvoiceViewModel.cs     # Invoice generation
│   ├── KhataLedgerViewModel.cs # Ledger display
│   ├── DailyScheduleViewModel.cs # Daily schedule
│   ├── SettingsViewModel.cs    # NEW 2.0: Settings management
│   └── ReportsViewModel.cs     # NEW 2.0: Analytics & export
│
├── Views/                      # UI xaml pages
│   ├── DashboardPage.xaml
│   ├── CustomersPage.xaml
│   ├── WorkOrdersPage.xaml
│   ├── InvoicePage.xaml
│   ├── KhataLedgerPage.xaml
│   ├── DailySchedulePage.xaml
│   ├── SettingsPage.xaml       # Pending UI creation
│   └── ReportsPage.xaml        # Pending UI creation
│
├── Converters/
│   └── Converters.cs           # Value converters for binding
│
├── Styles/
│   ├── Colors.xaml             # Material Design 5.0 colors
│   ├── Typography.xaml         # Font definitions
│   └── Glassmorphism.xaml      # Glass effect styles
│
└── Documentation/
    ├── QUICK_START.md          # 5-minute user setup
    ├── PRODUCTION_GUIDE.md     # Operations & deployment (500+ lines)
    ├── CHANGELOG.md            # v2.0.0 release notes (650+ lines)
    ├── TECH_STACK.md           # Technologies & versions
    ├── DESIGN_GUIDE.md         # UI design patterns
    ├── IMPLEMENTATION_PLAN.md  # v2.0.0 feature plan
    └── WORKFLOW.md             # Business process workflows
```

---

## Architecture Layers

### **Layer 1: Presentation (XAML/WPF)**
- Data templates, value converters
- Event binding to Commands
- Material Design themes
- Glassmorphism effects

### **Layer 2: ViewModel (MVVM)**
- Data binding properties (INotifyPropertyChanged)
- Relay commands (async)
- State management
- Error message display

### **Layer 3: Service (Business Logic)**
- 10 core services (infrastructure)
- 8 domain services (business operations)
- Interfaces for DI
- Consistent error handling

### **Layer 4: Data Access (EF Core)**
- DbContext (BillFlowContext)
- Entity mappings
- Navigation properties
- Query optimization

### **Layer 5: Database (SQLite)**
- 12 tables with relationships
- Indexes on foreign keys
- Transaction support
- Local-first storage

---

## Service Dependency Map

```
ILoggingService (CORE - used by all)
    ↓ injects into
    ├─ IValidationService
    ├─ ISettingsService
    ├─ ICreditAlertService
    ├─ IOfflineSyncService
    ├─ IBackupService
    ├─ INotificationService
    ├─ IExportImportService
    └─ All business services

ICalculationService (CORE - no dependencies)
    ↓
IDiscountService
    ↓
IWorkOrderService (depends on Validation, Logging, Ledger)

ICreditAlertService (depends on Ledger data)
    ↓
DashboardService

IOffsyncService (depends on SettingsService)
    ↓
BackupService (async housekeeping)
```

---

## Database Schema

### **Core Tables**

**Customer**
```sql
CREATE TABLE Customers (
    CustomerId INTEGER PRIMARY KEY,
    CustomerCode TEXT UNIQUE,
    Name TEXT NOT NULL,
    Phone TEXT,
    Email TEXT,
    Address TEXT,
    CreditLimit DECIMAL(10,2),
    TotalBalance DECIMAL(10,2),
    CreatedAt DATETIME,
    UpdatedAt DATETIME
);
CREATE INDEX idx_customer_code ON Customers(CustomerCode);
```

**WorkOrder**
```sql
CREATE TABLE WorkOrders (
    WorkOrderId INTEGER PRIMARY KEY,
    WorkOrderNumber TEXT UNIQUE,
    CustomerId INTEGER FOREIGN KEY,
    RequiredDate DATE,
    DeliveryDate DATE,
    Status TEXT,
    TotalAmount DECIMAL(10,2),
    TotalQuantity DECIMAL(10,2),
    CreatedAt DATETIME,
    UpdatedAt DATETIME
);
```

**WorkOrderLineItem**
```sql
CREATE TABLE WorkOrderLineItems (
    LineItemId INTEGER PRIMARY KEY,
    WorkOrderId INTEGER FOREIGN KEY,
    Description TEXT,
    Width DECIMAL(10,2),
    Height DECIMAL(10,2),
    Quantity DECIMAL(10,2),
    RatePerSqft DECIMAL(10,2),
    SqFt DECIMAL(10,1),
    Amount DECIMAL(10,2)
);
```

**Ledger**
```sql
CREATE TABLE Ledgers (
    LedgerId INTEGER PRIMARY KEY,
    CustomerId INTEGER FOREIGN KEY,
    TransactionType TEXT (Debit/Credit),
    Amount DECIMAL(10,2),
    Balance DECIMAL(10,2),
    Remarks TEXT,
    CreatedAt DATETIME
);
```

**Payment**
```sql
CREATE TABLE Payments (
    PaymentId INTEGER PRIMARY KEY,
    CustomerId INTEGER FOREIGN KEY,
    Amount DECIMAL(10,2),
    PaymentDate DATE,
    PaymentMethod TEXT,
    ReferenceNumber TEXT,
    CreatedAt DATETIME
);
```

**Invoice**
```sql
CREATE TABLE Invoices (
    InvoiceId INTEGER PRIMARY KEY,
    WorkOrderId INTEGER FOREIGN KEY,
    InvoiceNumber TEXT UNIQUE,
    InvoiceDate DATE,
    DueDate DATE,
    Amount DECIMAL(10,2),
    TaxAmount DECIMAL(10,2),
    TaxRate DECIMAL(5,2),
    Status TEXT (Draft/Sent/Paid),
    CreatedAt DATETIME
);
```

**Settings** (v2.0.0 Enhanced)
```sql
CREATE TABLE Settings (
    SettingId INTEGER PRIMARY KEY,
    CompanyName TEXT,
    CompanyPhone TEXT,
    CompanyEmail TEXT,
    CompanyAddress TEXT,
    DefaultRatePerSqFt DECIMAL(10,2),
    TaxRate DECIMAL(5,2),              -- NEW 2.0
    InvoiceHeader TEXT,                -- NEW 2.0
    InvoiceFooter TEXT,                -- NEW 2.0
    LastSyncDate DATETIME NULL,        -- NEW 2.0
    EnableCredit BOOLEAN,               -- NEW 2.0
    EnableAutoBackup BOOLEAN,           -- NEW 2.0
    EnableOfflineMode BOOLEAN,          -- NEW 2.0
    EnableCreditAlerts BOOLEAN,         -- NEW 2.0
    CreatedAt DATETIME,
    UpdatedAt DATETIME
);
```

**NEW v2.0.0 Tables**

**Discount**
```sql
CREATE TABLE Discounts (
    DiscountId INTEGER PRIMARY KEY,
    WorkOrderId INTEGER FOREIGN KEY,
    DiscountCode TEXT,
    PercentageOff DECIMAL(5,2),
    AmountOff DECIMAL(10,2),
    Reason TEXT,
    AppliedAt DATETIME,
    ApprovedByUserId TEXT
);
```

**NotificationMessage**
```sql
CREATE TABLE NotificationMessages (
    MessageId INTEGER PRIMARY KEY,
    CustomerId INTEGER FOREIGN KEY,
    NotificationType TEXT (SMS/WhatsApp/Email),
    RecipientPhone TEXT,
    MessageContent TEXT,
    SentAt DATETIME,
    Status TEXT (Pending/Sent/Failed),
    ErrorMessage TEXT,
    WorkOrderId INTEGER FOREIGN KEY NULL
);
```

**CreditAlert**
```sql
CREATE TABLE CreditAlerts (
    AlertId INTEGER PRIMARY KEY,
    CustomerId INTEGER FOREIGN KEY,
    AlertType TEXT,
    Message TEXT,
    CreatedAt DATETIME,
    AcknowledgedAt DATETIME NULL,
    IsActive BOOLEAN,
    RelatedAmount DECIMAL(10,2)
);
```

**SyncQueueItem**
```sql
CREATE TABLE SyncQueueItems (
    QueueId INTEGER PRIMARY KEY,
    EntityType TEXT,
    EntityId INTEGER,
    Operation TEXT (Create/Update/Delete),
    QueuedAt DATETIME,
    SyncedAt DATETIME NULL,
    SyncError TEXT,
    RetryCount INTEGER
);
```

---

## Key Patterns Used

### **1. Service Result Pattern**
All services return structured results instead of throwing exceptions:
```csharp
public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; }
    public T Data { get; set; }
}

// Usage
var result = await customerService.CreateAsync(new Customer { ... });
if (result.IsSuccess)
{
    var customer = result.Data;
}
else
{
    // Handle errors from result.Errors list
}
```

### **2. Dependency Injection Pattern**
All services injected via constructor:
```csharp
public class WorkOrderService
{
    private readonly BillFlowContext _context;
    private readonly IValidationService _validation;
    private readonly ILoggingService _logging;

    public WorkOrderService(
        BillFlowContext context,
        IValidationService validation,
        ILoggingService logging)
    {
        _context = context;
        _validation = validation;
        _logging = logging;
    }
}
```

### **3. MVVM Binding Pattern**
ViewModels expose INotifyPropertyChanged properties:
```csharp
private ObservableCollection<CustomerViewModel> _customers;
public ObservableCollection<CustomerViewModel> Customers
{
    get => _customers;
    set => SetProperty(ref _customers, value);
}

private IAsyncRelayCommand _loadDataCommand;
public IAsyncRelayCommand LoadDataCommand =>
    _loadDataCommand ??= new AsyncRelayCommand(LoadDataAsync);
```

### **4. Validation-First Pattern**
Validation happens in service layer before any database operation:
```csharp
public async Task<ServiceResult<Customer>> CreateAsync(Customer customer)
{
    // Validate first
    var validation = _validationService.ValidateCustomer(customer);
    if (!validation.IsSuccess)
        return new ServiceResult<Customer> { Errors = validation.Errors };

    // Then save
    _context.Customers.Add(customer);
    await _context.SaveChangesAsync();

    // Then log
    _logging.LogInfo($"Customer {customer.Name} created", "CustomerService");

    return new ServiceResult<Customer> { IsSuccess = true, Data = customer };
}
```

### **5. Async/Await Pattern**
All I/O operations are truly async:
```csharp
public async Task<ServiceResult<List<Customer>>> GetAllAsync()
{
    var customers = await _context.Customers
        .AsNoTracking()
        .ToListAsync();

    return new ServiceResult<List<Customer>>
    {
        IsSuccess = true,
        Data = customers
    };
}
```

---

## Common Development Tasks

### **Add a New Service**

1. Create `YourService.cs` in `Services/` folder
2. Define interface `IYourService`
3. Implement interface with DI constructor
4. Inject in `App.xaml.cs`:
```csharp
services.AddSingleton<IYourService, YourService>();
```
5. Use in ViewModels by injecting interface

### **Add Entity Validation**

1. Open `ValidationService.cs`
2. Add validation method:
```csharp
public ServiceResult ValidateYourEntity(YourEntity entity)
{
    var result = new ServiceResult();
    
    if (string.IsNullOrEmpty(entity.Name))
        result.Errors.Add("Name is required");
    
    if (entity.Amount <= 0)
        result.Errors.Add("Amount must be positive");
    
    result.IsSuccess = result.Errors.Count == 0;
    return result;
}
```
3. Call from service before database operation

### **Add Database Column**

1. Add property to entity in `Models.cs`
2. Update `BillFlowContext.cs` if needed for Index/Constraints
3. EF Core auto-migration on app startup creates the column
4. No manual SQL scripts needed

### **Add ViewModel Command**

1. Define command property with RelayCommand<T>:
```csharp
private IAsyncRelayCommand<Customer> _deleteCommand;
public IAsyncRelayCommand<Customer> DeleteCommand =>
    _deleteCommand ??= new AsyncRelayCommand<Customer>(DeleteAsync);
```
2. Implement async method:
```csharp
private async Task DeleteAsync(Customer customer)
{
    try
    {
        var result = await _service.DeleteAsync(customer.Id);
        if (result.IsSuccess)
            Customers.Remove(customer);
    }
    catch (Exception ex)
    {
        StatusMessage = ex.Message;
    }
}
```
3. Bind in XAML: `Command="{Binding DeleteCommand}" CommandParameter="{Binding SelectedCustomer}"`

### **Add Export Format**

1. Open `ExportImportService.cs`
2. Add export method:
```csharp
public async Task<ServiceResult<string>> ExportToXmlAsync(...)
{
    // Implement XML serialization
}
```
3. Wire up in `ReportsViewModel.cs`

---

## Testing Checklist

### **Unit Tests (Services)**
- [ ] ValidationService: All validation rules
- [ ] CalculationService: Precision decimals
- [ ] CreditAlertService: Alert generation
- [ ] DiscountService: Discount calculations

### **Integration Tests**
- [ ] Customer → WorkOrder → Invoice flow
- [ ] Payment → Ledger → CreditAlert flow
- [ ] Offline → Sync → Online flow
- [ ] Backup → Restore cycle

### **UI Tests**
- [ ] Navigation between pages
- [ ] Data binding updates
- [ ] Command execution
- [ ] Error message display

### **Performance Tests**
- [ ] 10,000+ customers list load
- [ ] Export 5,000+ records
- [ ] Database query optimization
- [ ] UI responsiveness with large datasets

---

## Deployment Steps

### **Pre-Build**
1. ✅ Verify all services registered in App.xaml.cs
2. ✅ Check database schema matches Models.cs
3. ✅ Run final unit tests
4. ✅ Update version (v2.0.0) in App.xaml

### **Build**
```powershell
cd BillFlowApp
dotnet publish -c Release -o .\publish
```

### **Package**
1. Copy `publish/BillFlow.exe` to installer
2. Copy `publish/` all dependencies
3. Create installer with WiX or Inno Setup
4. Include PRODUCTION_GUIDE.md and QUICK_START.md

### **Deploy**
1. Run installer on target machine
2. Database initializes automatically
3. Default settings created
4. App ready for first login

---

## Performance Optimizations

### **Current Optimizations**
- ✅ 30-minute settings cache (SettingsService)
- ✅ AsNoTracking() on read-only queries
- ✅ Database indexes on foreign keys
- ✅ In-memory log buffer (1000 entries)
- ✅ Pagination ready (WorkOrders/Customers)

### **Future Optimizations**
- 🔄 Add query pagination (50 records per page)
- 🔄 Compress backup files (gzip)
- 🔄 Implement view models vs full models
- 🔄 Add caching layer (Redis for future cloud)
- 🔄 Batch insert operations

---

## Troubleshooting Guide

| Issue | Debug | Solution |
|-------|-------|----------|
| App crashes on startup | Check logs: `%APPDATA%\BillFlow\Logs\` | Verify .NET 8.0 installed, check DB file permissions |
| Database locked | Check database file: `%LOCALAPPDATA%\BillFlow\` | Close other instances, ensure file not on network drive |
| Calculations wrong | Enable debug logging in SettingsService | Check decimal precision, verify formula in CalculationService |
| Offline sync fails | Check `SyncQueueItems` table | Verify internet connectivity, check sync error messages |
| Backup takes too long | Monitor file size | Implement compression, verify disk speed |

---

## Version Highway

| Version | Features | Status |
|---------|----------|--------|
| v1.0.0 | Basic CRUD, Dashboard | ✅ Complete |
| v2.0.0 | 10 services, offline-first, credit mgmt, backup | ✅ In Review |
| v3.0.0 | UI pages for v2.0, testing, deployment | ⏳ Planned |
| v4.0.0 | Cloud sync (optional), mobile app API | 🔮 Future |
| v5.0.0 | Multi-user, advanced analytics, AI pricing | 🔮 Future |

---

## Contributing Guidelines

### **Code Style**
- Use PascalCase for properties, methods, classes
- Use camelCase for local variables, parameters
- Add XML doc comments for public members
- Follow MVVM separation: no UI logic in ViewModels

### **Service Layer**
- Always return `ServiceResult<T>`, never throw (except critical app crashes)
- Inject logger, always log transactions
- Validate input before processing
- Include retry logic for external services

### **Database**
- Use async/await for all DB operations
- Use LINQ expressions, avoid SQL strings
- Add indexes to foreign keys
- Document new tables in PRODUCTION_GUIDE.md

### **XAML**
- Bind to ViewModel properties, not code-behind
- Use Material Design 5.0 resources
- Support responsive design (min 800x600)
- Handle null data gracefully

---

## Critical Files to Backup

Before major changes, backup these files:

1. `App.xaml.cs` - Service registration
2. `BillFlowContext.cs` - Database schema
3. `Models.cs` - Entity definitions
4. All files in `Services/` folder
5. `CHANGELOG.md` - Version history

---

## Support & References

- **EF Core:** https://learn.microsoft.com/en-us/ef/core/
- **WPF MVVM:** https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/
- **CommunityToolkit.Mvvm:** https://github.com/CommunityToolkit/dotnet
- **Material Design:** https://material.io/design

---

**Last Updated:** v2.0.0  
**Maintained By:** BillFlow Development Team  
**Next Review:** After v3.0.0 UI completion
