# BillFlow - Production Implementation Guide

**Version:** 2.0.0 (Production-Grade)  
**Last Updated:** April 6, 2026  
**Status:** Complete & Ready for Deployment

---

## Table of Contents

1. [System Overview](#system-overview)
2. [Implemented Features](#implemented-features)
3. [Architecture & Design Patterns](#architecture--design-patterns)
4. [Database Schema](#database-schema)
5. [Services Documentation](#services-documentation)
6. [Deployment Instructions](#deployment-instructions)
7. [Security & Best Practices](#security--best-practices)
8. [Troubleshooting](#troubleshooting)
9. [Maintenance & Support](#maintenance--support)

---

## System Overview

BillFlow is an enterprise-grade digital billing and credit management system for printing businesses. It replaces manual khata (ledger) books with an intelligent, automated, and offline-first platform.

### Key Statistics

- **Platform:** WPF (.NET 8.0 Windows)
- **Database:** SQLite (local-first, scalable to cloud)
- **UI Framework:** Material Design 5.0
- **MVVM Pattern:** CommunityToolkit.Mvvm
- **Architecture:** Service-oriented with dependency injection

---

## Implemented Features

### Core Features (Complete)

✅ **Customer Management**
- Add/Edit/Delete customers
- Phone-based search
- Credit limit management
- Payment history tracking
- Auto-generated customer codes

✅ **Work Order Management**
- Order creation with line items
- Automatic SqFt calculations
- Status tracking (Received → Completed)
- Payment status management
- Work scheduling by date

✅ **Digital Khata (Credit Ledger)**
- Real-time balance tracking
- Credit/Debit entry management
- Running balance calculations
- All-customers credit view
- Overdue payment tracking

✅ **Daily Schedule Management**
- Date-specific work filtering
- Done vs Pending split view
- Daily statistics (SqFt, Revenue)
- Quick status updates
- Customer grouping

✅ **Dashboard & Analytics**
- Real-time metrics display
- Top customers by balance
- Today's work summary
- Pending amount tracking
- Credit health indicators

✅ **Invoice Generation**
- PDF invoice creation
- Auto-numbering per customer
- Customizable terms
- Date stamping
- Print-ready format

---

### Enhanced Features (New - Production-Grade)

✅ **Validation & Error Handling**
- Comprehensive input validation
- Business rule enforcement
- Credit limit alerts
- Type-safe error messages
- Field-level validation feedback

✅ **Logging & Monitoring**
- Structured logging system
- File-based log storage
- Debug/Info/Warning/Error levels
- Operation tracking
- Performance diagnostics

✅ **Settings & Configuration**
- Company information management
- Tax rate configuration
- Default pricing settings
- Invoice customization
- Feature toggles (offline, backup, alerts)

✅ **Credit Alert System**
- Credit limit exceeded alerts
- Overdue payment tracking (30/60/90 days)
- High-risk customer identification
- Alert acknowledgment workflow
- Credit health reporting

✅ **Offline-First Sync**
- Automatic online/offline detection
- Operation queuing when offline
- Auto-sync when connectivity restored
- Retry mechanism (5 attempts)
- Sync status display

✅ **Backup & Recovery**
- Automatic daily backups
- On-demand backup creation
- Backup history tracking
- Local backup management
- Disaster recovery capability

✅ **Discount Management**
- Percentage-based discounts
- Flat amount discounts
- Discount code generation
- Audit trail (reason/date)
- Real-time total recalculation

✅ **Notifications (SMS/WhatsApp Ready)**
- Invoice notification templates
- Payment reminder system
- Credit alert notifications
- Failed notification retry
- Provider integration framework (Twilio-ready)

✅ **Enhanced Calculations**
- Multi-decimal precision
- Tax calculation support
- Discount application
- Running totals
- Formatted currency display

✅ **Data Export/Import**
- CSV export (all modules)
- JSON export (with relationships)
- CSV import with validation
- Batch operations
- Data format flexibility

✅ **Reports & Analytics**
- Credit health report
- Customer aging analysis
- Daily performance metrics
- Alert summary dashboard
- Export reports (CSV/JSON)

✅ **User Interface**
- Glassmorphism design system
- Responsive layouts
- Error message display
- Loading states
- Status indicators
- Consistent color scheme

---

## Architecture & Design Patterns

### Layered Architecture

```
┌─────────────────────────────┐
│   Presentation Layer        │  (XAML Pages + ViewModels)
│   (WPF UI)                  │
├─────────────────────────────┤
│   Business Logic Layer      │  (Services)
│   (Core Services)           │
├─────────────────────────────┤
│   Data Access Layer         │  (Entity Framework + DbContext)
│   (Entity Framework Core)   │
├─────────────────────────────┤
│   Database Layer            │  (SQLite)
│   (Local Storage)           │
└─────────────────────────────┘
```

### Design Patterns Used

1. **MVVM Pattern**
   - Separation of concerns
   - Data binding for UI reactivity
   - Command pattern for interactions

2. **Service Layer Pattern**
   - ICustomerService, IWorkOrderService, etc.
   - Dependency injection
   - Testability & modularity

3. **Repository Pattern** (via EF Core)
   - Abstract data access
   - DbContext as Unit of Work
   - Query optimization through includes

4. **Observer Pattern**
   - INotifyPropertyChanged
   - ObservableCollection
   - Real-time UI updates

5. **Decorator Pattern**
   - Validation wrapper
   - Logging wrapper
   - Error handling decoration

---

## Database Schema

### Core Tables

**Customers**
- Id (PK)
- CustomerCode (Unique)
- Name, Phone, Email, Address
- CreditLimit, CurrentBalance
- CreatedAt, LastPaymentDate

**WorkOrders**
- Id (PK)
- OrderNumber (Unique, indexed)
- CustomerId (FK)
- OrderDate, ScheduledDate, CompletedDate, DeliveredDate
- Status, PaymentStatus
- TotalAmount, AmountPaid, TotalSqFt
- IsSyncPending, LastModified

**LineItems**
- Id (PK)
- WorkOrderId (FK)
- Description
- Width, Height, Quantity
- AreaSqFt, RatePerSqFt, TotalPrice
- DisplayOrder

**LedgerEntries**
- Id (PK)
- CustomerId (FK)
- WorkOrderId (FK, nullable)
- Type (Credit/Debit)
- Amount, BalanceAfter
- Description, EntryDate
- IsSyncPending

**Payments**
- Id (PK)
- CustomerId (FK)
- WorkOrderId (FK, nullable)
- Amount, PaymentDate
- PaymentMethod, Notes
- IsSyncPending

**New Tables (Production)**

**Settings**
- Id (PK)
- CompanyName, CompanyPhone, CompanyEmail, CompanyAddress
- DefaultRatePerSqFt, TaxRate
- InvoiceHeader, InvoiceFooter, InvoiceTerms
- EnableCredit, EnableAutoBackup, EnableOfflineMode, EnableCreditAlerts
- LastBackupDate, LastSyncDate

**Discounts**
- Id (PK)
- WorkOrderId (FK)
- DiscountCode, PercentageOff, AmountOff
- Reason, AppliedAt, ApprovedByUserId

**NotificationMessages**
- Id (PK)
- CustomerId (FK)
- NotificationType (SMS/WhatsApp/Email)
- RecipientPhone, MessageContent
- SentAt, Status, ErrorMessage
- WorkOrderId (FK, nullable)

**CreditAlerts**
- Id (PK)
- CustomerId (FK)
- AlertType, Message
- CreatedAt, AcknowledgedAt
- IsActive, RelatedAmount

**SyncQueueItems**
- Id (PK)
- EntityType, EntityId
- Operation (Create/Update/Delete)
- QueuedAt, SyncedAt
- SyncError, RetryCount

---

## Services Documentation

### Core Services

#### 1. **IValidationService**
Comprehensive input validation with business rules.

```csharp
// Validates customer data
ServiceResult result = await validationService.ValidateCustomer(customer);

// Validates work order
ServiceResult result = await validationService.ValidateWorkOrder(order);

// Checks credit limit before applying charge
ServiceResult result = await validationService.ValidateCreditLimit(customer, amount);
```

**Validations Include:**
- Required field checks
- Length constraints
- Email format
- Phone number format
- Credit limit ranges
- Amount ranges
- Date constraints

---

#### 2. **ILoggingService**
Structured logging with file persistence and memory caching.

```csharp
// Log information
logger.LogInfo("Customer created", "Customer");

// Log warnings
logger.LogWarning("Credit limit exceeded", "Credit");

// Log errors
logger.LogError(exception, "ErrorCategory");
logger.LogError("Custom error message", exception, "Category");

// Debug logging
logger.LogDebug("Debug information", "Category");

// Retrieve recent logs
List<LogEntry> recent = logger.GetRecentLogs(100);

// Export all logs
string logExport = logger.ExportLogs();
```

**Log Location:** `%APPDATA%\BillFlow\Logs\billflow-yyyy-MM-dd.log`

---

#### 3. **ISettingsService**
Centralized configuration management with caching.

```csharp
// Get current settings
Settings settings = await settingsService.GetSettingsAsync();

// Update settings
ServiceResult result = await settingsService.UpdateSettingsAsync(settings);

// Get specific settings
decimal rate = await settingsService.GetDefaultRatePerSqFtAsync();
decimal taxRate = await settingsService.GetTaxRateAsync();
bool enableCredit = await settingsService.GetEnableCreditAsync();
```

---

#### 4. **ICalculationService**
Enhanced calculation engine with tax and discount support.

```csharp
// Calculate line item with tax
CalculationResult result = await calcService.CalculateLineItemAsync(
    lineItem, 
    taxRate: 5.0m, 
    discountPercent: 10.0m
);

// Calculate work order total
CalculationResult order = await calcService.CalculateWorkOrderTotalAsync(workOrderId);

// Individual calculations
decimal sqft = calcService.CalculateSquareFootage(14.3m, 13m, 1);
decimal cost = calcService.CalculateCost(sqft, 50m);
decimal tax = calcService.CalculateTax(cost, 5m);
decimal discount = calcService.CalculateDiscount(cost, 10m);
```

---

#### 5. **ICreditAlertService**
Credit monitoring and alert management.

```csharp
// Get active alerts
List<CreditAlert> alerts = await creditService.GetActiveAlertsAsync();

// Get alerts for specific customer
List<CreditAlert> customerAlerts = await creditService.GetAlertsForCustomerAsync(customerId);

// Create alert
await creditService.CreateAlertAsync(customerId, "CreditLimitExceeded", "Amount message");

// Acknowledge alert
await creditService.AcknowledgeAlertAsync(alertId);

// Get credit health report
CreditHealthReport report = await creditService.GetCreditHealthReportAsync();

// Check if credit limit exceeded
bool exceeded = await creditService.CheckCreditLimitExceededAsync(customerId);
```

---

#### 6. **IOfflineSyncService**
Offline-first sync infrastructure for disconnected scenarios.

```csharp
// Check if online
bool online = await syncService.IsOnlineAsync();

// Get sync status
SyncStatus status = await syncService.GetSyncStatusAsync();

// Get pending sync count
int pending = await syncService.GetPendingSyncCountAsync();

// Manual sync
await syncService.ManualSyncAsync();

// Get pending items
List<SyncQueueItem> items = await syncService.GetPendingItemsAsync();

// Auto-sync management
syncService.StartAutoSync();  // Syncs every 5 minutes
syncService.StopAutoSync();
```

**Auto-Sync Behavior:**
- Triggered every 5 minutes
- Only if device is online
- Retries failed items up to 5 times
- Automatic on app start

---

#### 7. **IBackupService**
Automated backup and disaster recovery.

```csharp
// Create backup
ServiceResult result = await backupService.CreateLocalBackupAsync();

// Restore from backup
ServiceResult result = await backupService.RestoreFromBackupAsync(filePath);

// Get backup history
List<BackupInfo> backups = await backupService.GetBackupHistoryAsync();

// Delete backup
ServiceResult result = await backupService.DeleteBackupAsync(filePath);

// Auto-backup management
bool enabled = await backupService.EnableAutoBackupAsync();   // Daily
bool disabled = await backupService.DisableAutoBackupAsync();

// Get backup status
BackupStatus status = await backupService.GetBackupStatusAsync();
```

**Backup Location:** `%APPDATA%\BillFlow\Backups\`

**Auto-Backup:** Daily at app startup (if enabled)

---

#### 8. **IDiscountService**
Discount management for work orders.

```csharp
// Apply discount
ServiceResult result = await discountService.ApplyDiscount(
    workOrderId, 
    percentageOff: 10m, 
    reason: "Bulk order discount"
);

// Remove discount
ServiceResult result = await discountService.RemoveDiscount(discountId);

// Get order discounts
List<Discount> discounts = await discountService.GetOrderDiscountsAsync(workOrderId);

// Lookup by code
Discount discount = await discountService.GetDiscountByCodeAsync("DISC-XYZ");

// Calculate discounted total
decimal total = await discountService.CalculateDiscountedTotalAsync(workOrderId);
```

---

#### 9. **INotificationService**
SMS/WhatsApp/Email notification system.

```csharp
// Send invoice notification
ServiceResult result = await notificationService.SendInvoiceAsync(
    customerId, 
    workOrderId, 
    "SMS"  // or "WhatsApp", "Email"
);

// Send payment reminder
ServiceResult result = await notificationService.SendPaymentReminderAsync(
    customerId, 
    "SMS"
);

// Send credit alert
ServiceResult result = await notificationService.SendCreditAlertAsync(
    customerId,
     "Your credit limit has been exceeded",
    "SMS"
);

// Get notification history
List<NotificationMessage> history = await notificationService.GetNotificationHistoryAsync(customerId, count: 50);

// Resend failed notifications
ServiceResult result = await notificationService.ResendFailedNotificationsAsync();
```

**Notification Status:** Pending → Sent/Failed → (optionally Resent)

---

#### 10. **IExportImportService**
Data import/export for migrations and backups.

```csharp
// Export to CSV
ServiceResult result = await exportService.ExportToCsvAsync(
    filePath: "C:\\exports\\customers.csv",
    entityType: "Customers"  // "Customers", "WorkOrders", "Payments", "Ledger"
);

// Export to JSON
ServiceResult result = await exportService.ExportToJsonAsync(
    filePath: "C:\\exports\\customers.json",
    entityType: "Customers"
);

// Import from CSV
ServiceResult result = await exportService.ImportFromCsvAsync(
    filePath: "C:\\imports\\customers.csv",
    entityType: "Customers"
);

// Import from JSON
ServiceResult result = await exportService.ImportFromJsonAsync(
    filePath: "C:\\imports\\customers.json",
    entityType: "Customers"
);

// Get supported types
List<string> types = await exportService.GetSupportedEntityTypesAsync();
```

---

## Deployment Instructions

### Prerequisites

- Windows 10 or later
- .NET 8.0 Runtime (or SDK for development)
- SQLite (included in package)
- 100 MB free disk space minimum

### Installation Steps

1. **Extract Files**
   ```
   Extract BillFlow-v2.0.0.zip to C:\Program Files\BillFlow\
   ```

2. **Create Shortcuts**
   ```
   Right-click BillFlow.exe → Create shortcut
   Place on Desktop or Start Menu
   ```

3. **First Run**
   - Application creates local database automatically
   - Initializes with default settings
   - Creates necessary directories in %APPDATA%

4. **Configure Settings**
   - Open Settings page
   - Enter company information
   - Set default rate per SqFt
   - Configure tax rate
   - Enable/disable features as needed

5. **Create Test Customer**
   - Navigate to Customers page
   - Add sample customer
   - Verify system functionality

### Uninstallation

To preserve data during uninstallation:
1. Backup files in `%APPDATA%\BillFlow\`
2. Uninstall BillFlow
3. Data remains in backup location

To completely remove:
1. Delete application folder
2. Delete `%APPDATA%\BillFlow\` directory

---

## Security & Best Practices

### Data Security

1. **Local Database Encryption** (Recommended)
   - Use Windows EFS for database files
   - Location: `%LOCALAPPDATA%\BillFlow\billflow.db`

2. **Backup Security**
   - Store backups in secure location
   - Encrypt backup files if containing sensitive data
   - Keep backup media secure

3. **Notification Service Security**
   - Use environment variables for API keys
   - Never hardcode credentials
   - Validate all external services

### Best Practices

1. **Regular Backups**
   - Enable auto-backup in settings
   - Verify backup integrity weekly
   - Store offsite backup copy

2. **Log Review**
   - Check logs weekly for anomalies
   - Archive old logs monthly
   - Review error logs immediately

3. **Data Validation**
   - Validate all manual entries
   - Use decimal precision for amounts
   - Verify calculations independently for large orders

4. **System Maintenance**
   - Restart application weekly
   - Check available disk space
   - Update Windows and .NET regularly
   - Monitor database file size

---

## Troubleshooting

### Common Issues

**Issue:** Application won't start
- **Solution:** Verify .NET 8.0 runtime is installed
- Run: `dotnet --version`
- Reinstall .NET 8.0 if needed

**Issue:** Database locked error
- **Solution:** Close other instances of app
- Wait 30 seconds
- Restart application

**Issue:** Offline sync not working
- **Solution:** Check internet connection
- Verify VPN isn't blocking
- Check proxy settings
- Review sync status in Settings

**Issue:** Backup creation fails
- **Solution:** Check disk space (min 200 MB)
- Run application as administrator
- Verify `%APPDATA%\BillFlow\` has write permissions
- Check antivirus isn't blocking access

**Issue:** Notifications not sending
- **Solution:** Verify customer phone numbers include country code
- Check notification service configuration
- Review "Failed" messages in notification history
- Verify SMS service credentials

### Log Analysis

Access logs in Windows Explorer:
```
%APPDATA%\BillFlow\Logs\
```

Example log analysis:
```
[2026-04-06 14:32:15.123] [ERROR] [WorkOrder] InvalidOperationException: ...
→ Check work order validation rules
→ Verify customer exists
```

---

## Maintenance & Support

### Regular Maintenance Schedule

**Daily:**
- Check for error logs
- Monitor pending syncs

**Weekly:**
- Verify backups completed
- Review customer credit limits
- Check for failed notifications

**Monthly:**
- Archive old logs
- Review credit alerts
- Analyze reports

**Quarterly:**
- Database maintenance (rebuild indexes)
- Full system backup
- Security update check

### Performance Optimization

**For Large Customer Base (1000+):**
1. Archive completed orders older than 2 years
2. Use Reports export for historical analysis
3. Implement manual sync schedule instead of auto-sync
4. Consider SQLite WAL (Write-Ahead Logging) mode

**SQL Optimization:**
```sql
-- Enable WAL mode for better concurrency
PRAGMA journal_mode=WAL;

-- Optimize queries
ANALYZE;

-- Rebuild indexes
REINDEX;
```

---

## Version History

### v2.0.0 (Current) - April 6, 2026
- ✨ Complete production-grade implementation
- ✅ All core features implemented
- ✅ Enhanced validation & error handling
- ✅ Comprehensive logging system
- ✅ Offline-first sync infrastructure
- ✅ Credit alert system
- ✅ Export/import functionality
- ✅ Settings management
- ✅ Reports & analytics

### v1.0.0 - Initial Release
- Basic customer management
- Work order tracking
- Invoice generation
- Digital khata ledger

---

## Support & Contact

For technical support:
1. Check troubleshooting guide above
2. Review application logs
3. Export relevant data
4. Contact development team with:
   - Screenshots/logs
   - Steps to reproduce
   - Error messages

---

## License & Terms

**BillFlow** - Enterprise Edition v2.0.0
- Licensed for authorized printing business use
- Not for resale or redistribution
- Support available during business hours

---

**Document End**

*This is a production-grade, complete implementation of BillFlow. All features are tested and ready for deployment. Refer to this guide for configuration, operation, and maintenance.*
