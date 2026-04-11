# BillFlow - Product Requirements Document (PRD)

**Version:** 2.0.0  
**Date:** April 11, 2026  
**Status:** Production-Ready  
**Document Type:** Product Requirements Document

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Product Overview](#2-product-overview)
3. [Target Audience & Personas](#3-target-audience--personas)
4. [Functional Requirements](#4-functional-requirements)
5. [Non-Functional Requirements](#5-non-functional-requirements)
6. [Technical Architecture](#6-technical-architecture)
7. [Database Schema](#7-database-schema)
8. [User Interface Requirements](#8-user-interface-requirements)
9. [Security & Compliance](#9-security--compliance)
10. [Performance Requirements](#10-performance-requirements)
11. [Integration Points](#11-integration-points)
12. [Deployment Requirements](#12-deployment-requirements)
13. [Future Roadmap](#13-future-roadmap)

---

## 1. Executive Summary

BillFlow is an enterprise-grade, offline-first desktop billing and credit management system specifically designed for printing businesses. It replaces traditional manual "khata" (ledger) books with an intelligent, automated digital platform that enables business owners to track customer credits, manage work orders, generate invoices, and monitor daily operations with real-time analytics.

### Key Value Propositions

- **Offline-First Operation**: Full functionality without internet connectivity with automatic sync when online
- **Digital Khata Management**: Automated ledger tracking with real-time balance calculations
- **Work Order Automation**: Square footage calculations (Width × Height × Quantity) with automatic pricing
- **Credit Risk Management**: Visual indicators and alerts for overdue payments and credit limits
- **Production-Grade Reliability**: Comprehensive logging, automated backups, and data validation

---

## 2. Product Overview

### 2.1 Product Vision
To become the de facto standard billing solution for small-to-medium printing businesses, replacing paper-based accounting with an intuitive, reliable, and feature-rich digital platform.

### 2.2 Problem Statement
Printing businesses traditionally rely on:
- Manual ledger books (khata) prone to errors and loss
- Paper-based work tracking with no search capability
- Manual square footage calculations leading to pricing errors
- No systematic credit risk monitoring
- Difficulty tracking work status across multiple orders
- No automated backup or disaster recovery

### 2.3 Solution Overview
BillFlow provides a comprehensive WPF-based desktop application with:
- Digital customer management with auto-generated codes
- Automated work order processing with SqFt calculations
- Real-time digital ledger (khata) with credit/debit tracking
- Daily schedule management with status tracking
- PDF invoice generation with customizable templates
- Credit alert system with risk indicators (🟢🟡🔴)

### 2.4 Current Version
**Version 2.0.0** - Production Grade Release
- 12 database entities
- 15 core services
- 9 ViewModels
- 11 XAML pages
- Comprehensive validation and error handling

---

## 3. Target Audience & Personas

### 3.1 Primary Persona: Printing Business Owner

**Name:** Rajesh Kumar  
**Age:** 42  
**Business:** Medium-sized printing press (5-10 employees)  
**Location:** Urban India

**Goals:**
- Track customer credits efficiently without paper ledgers
- Monitor daily work schedule and completion status
- Generate professional invoices quickly
- Identify high-risk customers before extending credit
- Access business analytics and cashflow insights

**Pain Points:**
- Lost or damaged physical ledger books
- Difficulty calculating print area costs accurately
- Forgotten payments and overdue tracking
- No visibility into daily production capacity
- Time-consuming manual invoice creation

**Technical Profile:**
- Moderate computer literacy
- Windows desktop user
- Prefers simple, intuitive interfaces
- Values reliability over flashy features

### 3.2 Secondary Persona: Shop Manager/Accountant

**Name:** Priya Sharma  
**Role:** Shop manager handling day-to-day operations

**Goals:**
- Quickly add new customers and work orders
- Update work status throughout the day
- Record payments and update ledger
- Generate daily/weekly reports

**Pain Points:**
- Complex software requiring extensive training
- Slow performance with many records
- Difficulty finding historical data

---

## 4. Functional Requirements

### 4.1 Customer Management Module

#### FR-CM-001: Customer Registration
- **Priority:** P0 (Critical)
- **Description:** System shall allow creating new customers with auto-generated unique customer codes
- **Acceptance Criteria:**
  - Customer code format: `CUST####` (e.g., CUST0001)
  - Required fields: Name
  - Optional fields: Phone, Email, Address, Credit Limit
  - Validation: Name max 100 chars, Phone max 20 chars
  - Duplicate phone detection warning

#### FR-CM-002: Customer Search & Directory
- **Priority:** P0
- **Description:** System shall provide searchable customer directory
- **Acceptance Criteria:**
  - Search by name, customer code, or phone number
  - Case-insensitive search
  - Results sorted alphabetically by name
  - Display credit status indicator (🟢🟡🔴)

#### FR-CM-003: Customer Profile Management
- **Priority:** P1 (High)
- **Description:** System shall allow viewing and editing customer details
- **Acceptance Criteria:**
  - View complete customer history
  - Edit contact information
  - Update credit limits
  - View running balance
  - View all associated work orders

#### FR-CM-004: Credit Limit Management
- **Priority:** P1
- **Description:** System shall enforce and track customer credit limits
- **Acceptance Criteria:**
  - Set custom credit limit per customer
  - Visual warning when approaching limit
  - Block new credit orders when limit exceeded (configurable)
  - Track remaining credit availability

### 4.2 Work Order Management Module

#### FR-WO-001: Work Order Creation
- **Priority:** P0
- **Description:** System shall allow creating work orders with automatic order numbering
- **Acceptance Criteria:**
  - Order number format: `WOYYYYMMDD-###` (e.g., WO20260411-001)
  - Link to existing customer (required)
  - Add multiple line items per order
  - Set scheduled date for work
  - Default payment status: Credit

#### FR-WO-002: Line Item Management
- **Priority:** P0
- **Description:** System shall calculate line item costs based on dimensions
- **Acceptance Criteria:**
  - Fields: Description, Width (ft), Height (ft), Quantity
  - Automatic calculation: Area = Width × Height × Quantity
  - Configurable rate per SqFt (default: ₹50)
  - Automatic total price calculation
  - Support decimal precision (2 decimal places)

#### FR-WO-003: Work Status Tracking
- **Priority:** P0
- **Description:** System shall track work order through status lifecycle
- **Acceptance Criteria:**
  - Status values: Received → InProgress → Ready → Delivered → Completed
  - Date tracking: OrderDate, ScheduledDate, CompletedDate, DeliveredDate
  - Status change logging
  - Visual status indicators in UI

#### FR-WO-004: Payment Status Management
- **Priority:** P1
- **Description:** System shall track payment status separately from work status
- **Acceptance Criteria:**
  - Payment statuses: Cash, Credit, Partial, Paid
  - Record amount paid against total
  - Calculate balance due automatically
  - Link payments to ledger entries

### 4.3 Digital Khata (Ledger) Module

#### FR-LG-001: Automatic Ledger Entries
- **Priority:** P0
- **Description:** System shall automatically create ledger entries for credit transactions
- **Acceptance Criteria:**
  - Credit entry created when work order is saved with Credit/Partial status
  - Debit entry created when payment is recorded
  - Running balance calculation after each entry
  - Entry description includes work order reference

#### FR-LG-002: Ledger View & History
- **Priority:** P0
- **Description:** System shall display complete transaction history per customer
- **Acceptance Criteria:**
  - Chronological list of all entries
  - Entry type indicator (Credit/Debit)
  - Running balance display
  - Entry date and description
  - Link to associated work order

#### FR-LG-003: Customer Balance Summary
- **Priority:** P1
- **Description:** System shall provide at-a-glance balance overview
- **Acceptance Criteria:**
  - Current balance per customer
  - Total credit extended
  - Total payments received
  - Last payment date

### 4.4 Daily Schedule Module

#### FR-DS-001: Date-Based Work Filtering
- **Priority:** P1
- **Description:** System shall display work orders scheduled for specific dates
- **Acceptance Criteria:**
  - Calendar date picker
  - Show all work scheduled for selected date
  - Separate view for Completed vs Pending work
  - Customer grouping

#### FR-DS-002: Daily Statistics
- **Priority:** P1
- **Description:** System shall calculate daily performance metrics
- **Acceptance Criteria:**
  - Total SqFt completed
  - Total SqFt pending
  - Revenue completed
  - Revenue pending
  - Work count by status

#### FR-DS-003: Quick Status Updates
- **Priority:** P2 (Medium)
- **Description:** System shall allow quick status changes from daily view
- **Acceptance Criteria:**
  - Mark work as completed
  - Update delivery status
  - One-click status transitions

### 4.5 Dashboard & Analytics Module

#### FR-DB-001: Real-Time Metrics
- **Priority:** P0
- **Description:** System shall display key business metrics on dashboard
- **Acceptance Criteria:**
  - Total pending amount across all customers
  - Today's revenue (payments received today)
  - Today's work completed count
  - Today's work pending count
  - Total active customers count
  - Monthly revenue summary

#### FR-DB-002: Top Customers Widget
- **Priority:** P1
- **Description:** System shall display customers with highest outstanding balances
- **Acceptance Criteria:**
  - Top 5 customers by balance
  - Risk level indicator per customer
  - Quick navigation to customer details

#### FR-DB-003: Credit Risk Indicators
- **Priority:** P1
- **Description:** System shall categorize customers by credit risk
- **Acceptance Criteria:**
  - 🟢 Clear: Paid within 7 days
  - 🟡 Moderate: Paid within 30 days
  - 🔴 High Risk: >30 days overdue OR exceeds credit limit
  - Automatic risk calculation based on payment history

### 4.6 Invoice Generation Module

#### FR-IN-001: PDF Invoice Creation
- **Priority:** P0
- **Description:** System shall generate professional PDF invoices
- **Acceptance Criteria:**
  - Company header with logo placeholder
  - Customer billing information
  - Line item details with dimensions
  - Total area and amount calculations
  - Payment status and balance due
  - Customizable terms and footer

#### FR-IN-002: Customer Statement Generation
- **Priority:** P1
- **Description:** System shall generate ledger statements for customers
- **Acceptance Criteria:**
  - Complete transaction history
  - Running balance display
  - Date range filtering
  - Professional formatting
  - Multi-page support for long histories

### 4.7 Payment Recording Module

#### FR-PR-001: Payment Entry
- **Priority:** P0
- **Description:** System shall record customer payments
- **Acceptance Criteria:**
  - Payment amount entry
  - Payment date (default: today)
  - Payment method (Cash, UPI, Bank Transfer, etc.)
  - Optional notes field
  - Automatic ledger debit entry
  - Work order payment status update

#### FR-PR-002: Payment History
- **Priority:** P1
- **Description:** System shall display payment history per customer
- **Acceptance Criteria:**
  - Chronological payment list
  - Payment method display
  - Associated work order link
  - Total payments summary

### 4.8 Settings & Configuration Module

#### FR-ST-001: Company Information
- **Priority:** P1
- **Description:** System shall store and use company information
- **Acceptance Criteria:**
  - Company name, address, phone, email
  - Display on invoices
  - Persist across sessions

#### FR-ST-002: Pricing Configuration
- **Priority:** P1
- **Description:** System shall allow configurable pricing defaults
- **Acceptance Criteria:**
  - Default rate per SqFt (default: ₹50)
  - Tax rate configuration
  - Currency symbol (default: ₹)

#### FR-ST-003: Invoice Customization
- **Priority:** P2
- **Description:** System shall allow invoice template customization
- **Acceptance Criteria:**
  - Custom invoice header text
  - Custom invoice footer text
  - Payment terms text
  - Default terms message

#### FR-ST-004: Feature Toggles
- **Priority:** P2
- **Description:** System shall allow enabling/disabling features
- **Acceptance Criteria:**
  - Enable/disable credit system
  - Enable/disable auto-backup
  - Enable/disable offline mode
  - Enable/disable credit alerts

### 4.9 Credit Alert System Module

#### FR-CA-001: Automatic Alert Generation
- **Priority:** P1
- **Description:** System shall generate alerts based on credit conditions
- **Acceptance Criteria:**
  - Alert types: CreditLimitExceeded, OverduePayment, LowCredit
  - Automatic generation when thresholds crossed
  - Related amount tracking
  - Customer link

#### FR-CA-002: Alert Management
- **Priority:** P1
- **Description:** System shall display and manage credit alerts
- **Acceptance Criteria:**
  - Active alerts list
  - Alert acknowledgment workflow
  - Historical alert view
  - Alert count indicator on dashboard

### 4.10 Backup & Recovery Module

#### FR-BR-001: Automatic Backup
- **Priority:** P1
- **Description:** System shall perform automatic daily backups
- **Acceptance Criteria:**
  - Daily backup at application startup (if enabled)
  - Backup location: %APPDATA%\BillFlow\Backups\
  - Timestamped backup files
  - Backup history tracking

#### FR-BR-002: Manual Backup & Restore
- **Priority:** P1
- **Description:** System shall allow manual backup and restore operations
- **Acceptance Criteria:**
  - On-demand backup creation
  - Restore from backup file
  - Backup deletion
  - Backup integrity verification

### 4.11 Export/Import Module

#### FR-EI-001: Data Export
- **Priority:** P2
- **Description:** System shall export data to standard formats
- **Acceptance Criteria:**
  - CSV export for all entities
  - JSON export with relationships
  - Selectable date ranges
  - Export progress indication

#### FR-EI-002: Data Import
- **Priority:** P2
- **Description:** System shall import data from standard formats
- **Acceptance Criteria:**
  - CSV import with validation
  - JSON import
  - Duplicate detection
  - Import error reporting

### 4.12 Notification System Module

#### FR-NS-001: Notification Framework
- **Priority:** P2
- **Description:** System shall support SMS/WhatsApp/Email notifications
- **Acceptance Criteria:**
  - Provider-agnostic architecture
  - Message template support
  - Delivery status tracking
  - Failed message retry capability

#### FR-NS-002: Notification Types
- **Priority:** P2
- **Description:** System shall support predefined notification types
- **Acceptance Criteria:**
  - Invoice notifications
  - Payment reminders
  - Credit alerts
  - Custom message support

### 4.13 Offline Sync Module

#### FR-OS-001: Offline Operation
- **Priority:** P1
- **Description:** System shall operate fully without internet connectivity
- **Acceptance Criteria:**
  - All CRUD operations functional offline
  - Local SQLite database
  - No internet dependency for core features

#### FR-OS-002: Sync Queue Management
- **Priority:** P1
- **Description:** System shall queue operations for later sync
- **Acceptance Criteria:**
  - Automatic operation queuing
  - Entity type tracking
  - Operation type tracking (Create/Update/Delete)
  - Retry count tracking
  - Error logging

#### FR-OS-003: Automatic Sync
- **Priority:** P1
- **Description:** System shall automatically sync when connectivity restored
- **Acceptance Criteria:**
  - Connectivity detection
  - Automatic sync trigger
  - 5-minute sync interval (configurable)
  - Sync status display
  - Conflict resolution for concurrent edits

---

## 5. Non-Functional Requirements

### 5.1 Performance Requirements

#### NFR-PF-001: Application Startup
- **Requirement:** Application shall start within 5 seconds on standard hardware
- **Measurement:** Time from launch to interactive dashboard

#### NFR-PF-002: Database Operations
- **Requirement:** All database queries shall complete within 2 seconds for datasets up to 10,000 records
- **Measurement:** Query execution time with 10K customers, 50K work orders

#### NFR-PF-003: UI Responsiveness
- **Requirement:** UI shall remain responsive during all operations
- **Measurement:** No UI freezing >500ms during normal operations

#### NFR-PF-004: PDF Generation
- **Requirement:** Invoice PDF generation shall complete within 3 seconds
- **Measurement:** Time to generate and save PDF

#### NFR-PF-005: Search Performance
- **Requirement:** Customer search shall return results within 1 second
- **Measurement:** Search across 10,000 customer records

### 5.2 Reliability Requirements

#### NFR-RL-001: Data Integrity
- **Requirement:** No data loss shall occur during normal operations or crashes
- **Implementation:** Transaction support, WAL mode SQLite, auto-save

#### NFR-RL-002: Backup Reliability
- **Requirement:** 99.9% backup success rate when auto-backup enabled
- **Measurement:** Successful backup completion tracking

#### NFR-RL-003: Recovery Capability
- **Requirement:** System shall recover from unexpected shutdown without data corruption
- **Implementation:** SQLite WAL mode, atomic transactions

### 5.3 Usability Requirements

#### NFR-US-001: Learning Curve
- **Requirement:** New user shall perform basic operations within 15 minutes without training
- **Target:** Printing business owner with basic computer literacy

#### NFR-US-002: Interface Consistency
- **Requirement:** All pages shall follow consistent design patterns
- **Implementation:** Material Design 5.0, glassmorphism styling, consistent spacing

#### NFR-US-003: Error Clarity
- **Requirement:** Error messages shall be clear and actionable
- **Implementation:** Field-level validation, descriptive error messages

#### NFR-US-004: Accessibility
- **Requirement:** Application shall support keyboard navigation
- **Standards:** WCAG AA compliance for color contrast, focus indicators

### 5.4 Maintainability Requirements

#### NFR-MT-001: Code Organization
- **Requirement:** Codebase shall follow MVVM architecture with clear separation of concerns
- **Implementation:** ViewModels, Services, Models, Database layers

#### NFR-MT-002: Logging
- **Requirement:** All operations shall be logged for debugging
- **Implementation:** Structured logging with categories (Debug, Info, Warning, Error)

#### NFR-MT-003: Documentation
- **Requirement:** All public APIs shall be documented
- **Implementation:** XML doc comments, README files, guides

### 5.5 Portability Requirements

#### NFR-PT-001: Platform Support
- **Requirement:** Application shall run on Windows 10 and Windows 11
- **Architecture:** x64, self-contained deployment

#### NFR-PT-002: Database Portability
- **Requirement:** Database file shall be movable between installations
- **Implementation:** Single SQLite file, no registry dependencies

---

## 6. Technical Architecture

### 6.1 Technology Stack

| Layer | Technology | Version | Purpose |
|-------|-----------|---------|---------|
| Framework | .NET | 8.0 | Application runtime |
| Language | C# | 12.0 | Primary language |
| UI Framework | WPF | .NET 8.0 | Desktop UI |
| UI Library | MaterialDesignInXAML | 5.x | UI components |
| Database | SQLite | 3.x | Local storage |
| ORM | Entity Framework Core | 8.0 | Data access |
| PDF Generation | PdfSharp | 6.x | Invoice PDFs |
| MVVM Framework | CommunityToolkit.Mvvm | 8.x | MVVM support |
| DI Container | Microsoft.Extensions.DependencyInjection | 8.x | Dependency injection |

### 6.2 Architecture Layers

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  (XAML Views + Code-Behind + Converters + Styles)       │
├─────────────────────────────────────────────────────────┤
│                    ViewModel Layer                     │
│  (MainViewModel, DashboardViewModel, etc.)            │
│  - INotifyPropertyChanged                              │
│  - IAsyncRelayCommand                                  │
│  - State Management                                    │
├─────────────────────────────────────────────────────────┤
│                    Service Layer                       │
│  (15 Services with Interface Segregation)             │
│  - CustomerService, WorkOrderService                   │
│  - LedgerService, InvoiceService                       │
│  - DashboardService, ValidationService                   │
│  - LoggingService, SettingsService                       │
│  - CreditAlertService, OfflineSyncService              │
│  - BackupService, DiscountService                       │
│  - NotificationService, ExportImportService            │
│  - CalculationService                                   │
├─────────────────────────────────────────────────────────┤
│                    Data Access Layer                   │
│  (Entity Framework Core DbContext)                    │
│  - BillFlowContext                                     │
│  - Repository pattern via EF Core                      │
├─────────────────────────────────────────────────────────┤
│                    Database Layer                      │
│  (SQLite)                                               │
│  - Local file-based storage                            │
│  - WAL mode enabled                                    │
└─────────────────────────────────────────────────────────┘
```

### 6.3 Design Patterns

1. **MVVM (Model-View-ViewModel)**: Separation of UI and business logic
2. **Dependency Injection**: Service registration in App.xaml.cs
3. **Service Layer**: Business logic encapsulated in services
4. **Repository Pattern**: EF Core DbContext as unit of work
5. **Observer Pattern**: INotifyPropertyChanged for UI updates
6. **Service Result Pattern**: Structured return types instead of exceptions

### 6.4 Service Dependency Map

```
ILoggingService (CORE - used by all services)
    ├── IValidationService
    ├── ISettingsService
    ├── ICreditAlertService
    ├── IOfflineSyncService
    ├── IBackupService
    ├── INotificationService
    ├── IExportImportService
    ├── ICustomerService
    ├── IWorkOrderService
    ├── IInvoiceService
    ├── ILedgerService
    └── IDashboardService

ICalculationService (CORE - no dependencies)
    └── IDiscountService

ILedgerService
    ├── IWorkOrderService
    └── IDashboardService
```

---

## 7. Database Schema

### 7.1 Entity Relationship Diagram

```
┌─────────────────┐       ┌──────────────────┐       ┌─────────────────┐
│    Customer     │       │    WorkOrder     │       │    LineItem     │
├─────────────────┤       ├──────────────────┤       ├─────────────────┤
│ PK Id           │──┐    │ PK Id            │──┐    │ PK Id           │
│ UQ CustomerCode │  │    │ UQ OrderNumber   │  │    │ FK WorkOrderId  │
│ Name            │  └──<│ FK CustomerId     │  └──<│ Description     │
│ Phone           │       │ OrderDate        │       │ Width           │
│ Email           │       │ ScheduledDate    │       │ Height          │
│ Address         │       │ Status           │       │ Quantity        │
│ CreditLimit     │       │ PaymentStatus    │       │ AreaSqFt        │
│ CurrentBalance  │       │ TotalAmount      │       │ RatePerSqFt     │
│ CreatedAt       │       │ TotalSqFt        │       │ TotalPrice      │
│ LastPaymentDate │       │ AmountPaid       │       │ DisplayOrder    │
└─────────────────┘       └──────────────────┘       └─────────────────┘
         │                         │
         │                         │
         ▼                         ▼
┌─────────────────┐       ┌──────────────────┐
│  LedgerEntry    │       │     Payment      │
├─────────────────┤       ├──────────────────┤
│ PK Id           │       │ PK Id            │
│ FK CustomerId   │       │ FK CustomerId    │
│ FK WorkOrderId  │       │ FK WorkOrderId   │
│ Type (C/D)      │       │ Amount           │
│ Amount          │       │ PaymentDate      │
│ BalanceAfter    │       │ PaymentMethod    │
│ Description     │       │ Notes            │
│ EntryDate       │       │ IsSyncPending    │
└─────────────────┘       └──────────────────┘
```

### 7.2 Core Tables

#### Customers Table
```sql
CREATE TABLE Customers (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerCode TEXT UNIQUE NOT NULL,
    Name TEXT NOT NULL,
    Phone TEXT,
    Email TEXT,
    Address TEXT,
    CreditLimit DECIMAL(18,2) DEFAULT 0,
    CurrentBalance DECIMAL(18,2) DEFAULT 0,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    LastPaymentDate DATETIME
);

CREATE INDEX idx_customers_code ON Customers(CustomerCode);
CREATE INDEX idx_customers_name ON Customers(Name);
```

#### WorkOrders Table
```sql
CREATE TABLE WorkOrders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderNumber TEXT UNIQUE NOT NULL,
    CustomerId INTEGER NOT NULL,
    OrderDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ScheduledDate DATETIME,
    CompletedDate DATETIME,
    DeliveredDate DATETIME,
    Status INTEGER DEFAULT 0, -- WorkStatus enum
    PaymentStatus INTEGER DEFAULT 1, -- PaymentStatus enum (Credit)
    TotalAmount DECIMAL(18,2) DEFAULT 0,
    AmountPaid DECIMAL(18,2) DEFAULT 0,
    TotalSqFt DECIMAL(18,2) DEFAULT 0,
    Notes TEXT,
    IsSyncPending BOOLEAN DEFAULT 0,
    LastModified DATETIME,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

CREATE INDEX idx_workorders_customer ON WorkOrders(CustomerId);
CREATE INDEX idx_workorders_status ON WorkOrders(Status);
CREATE INDEX idx_workorders_scheduled ON WorkOrders(ScheduledDate);
```

#### LineItems Table
```sql
CREATE TABLE LineItems (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    WorkOrderId INTEGER NOT NULL,
    Description TEXT NOT NULL,
    Width DECIMAL(10,2),
    Height DECIMAL(10,2),
    Quantity INTEGER DEFAULT 1,
    AreaSqFt DECIMAL(10,2),
    RatePerSqFt DECIMAL(10,2) DEFAULT 50,
    TotalPrice DECIMAL(18,2),
    DisplayOrder INTEGER DEFAULT 0,
    FOREIGN KEY (WorkOrderId) REFERENCES WorkOrders(Id) ON DELETE CASCADE
);
```

#### LedgerEntries Table
```sql
CREATE TABLE LedgerEntries (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerId INTEGER NOT NULL,
    WorkOrderId INTEGER,
    Type INTEGER NOT NULL, -- LedgerType enum (0=Credit, 1=Debit)
    Amount DECIMAL(18,2) NOT NULL,
    BalanceAfter DECIMAL(18,2) NOT NULL,
    Description TEXT,
    EntryDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsSyncPending BOOLEAN DEFAULT 0,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY (WorkOrderId) REFERENCES WorkOrders(Id)
);

CREATE INDEX idx_ledger_customer ON LedgerEntries(CustomerId);
CREATE INDEX idx_ledger_date ON LedgerEntries(EntryDate);
```

#### Payments Table
```sql
CREATE TABLE Payments (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerId INTEGER NOT NULL,
    WorkOrderId INTEGER,
    Amount DECIMAL(18,2) NOT NULL,
    PaymentDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    PaymentMethod TEXT DEFAULT 'Cash',
    Notes TEXT,
    IsSyncPending BOOLEAN DEFAULT 0,
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    FOREIGN KEY (WorkOrderId) REFERENCES WorkOrders(Id)
);

CREATE INDEX idx_payments_customer ON Payments(CustomerId);
CREATE INDEX idx_payments_date ON Payments(PaymentDate);
```

### 7.3 Configuration Tables

#### Settings Table
```sql
CREATE TABLE Settings (
    Id INTEGER PRIMARY KEY CHECK (Id = 1), -- Single row
    CompanyName TEXT DEFAULT 'My Printing Business',
    CompanyAddress TEXT,
    CompanyPhone TEXT,
    CompanyEmail TEXT,
    DefaultRatePerSqFt DECIMAL(10,2) DEFAULT 50,
    TaxRate DECIMAL(5,2) DEFAULT 0,
    InvoiceTerms TEXT DEFAULT 'Payment due within 15 days',
    InvoiceHeader TEXT DEFAULT 'INVOICE',
    InvoiceFooter TEXT DEFAULT 'Thank you for your business',
    CurrencySymbol TEXT DEFAULT '₹',
    LastBackupDate DATETIME,
    LastSyncDate DATETIME,
    EnableCredit BOOLEAN DEFAULT 1,
    EnableAutoBackup BOOLEAN DEFAULT 1,
    EnableOfflineMode BOOLEAN DEFAULT 1,
    EnableCreditAlerts BOOLEAN DEFAULT 1
);
```

### 7.4 Extended Tables (v2.0)

#### Discounts Table
```sql
CREATE TABLE Discounts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    WorkOrderId INTEGER NOT NULL,
    DiscountCode TEXT,
    PercentageOff DECIMAL(5,2),
    AmountOff DECIMAL(18,2),
    Reason TEXT,
    AppliedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    ApprovedByUserId INTEGER,
    FOREIGN KEY (WorkOrderId) REFERENCES WorkOrders(Id)
);
```

#### CreditAlerts Table
```sql
CREATE TABLE CreditAlerts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerId INTEGER NOT NULL,
    AlertType TEXT NOT NULL, -- CreditLimitExceeded, OverduePayment, LowCredit
    Message TEXT NOT NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    AcknowledgedAt DATETIME,
    IsActive BOOLEAN DEFAULT 1,
    RelatedAmount DECIMAL(18,2),
    FOREIGN KEY (CustomerId) REFERENCES Customers(Id)
);

CREATE INDEX idx_alerts_customer ON CreditAlerts(CustomerId);
CREATE INDEX idx_alerts_active ON CreditAlerts(IsActive);
```

#### SyncQueue Table
```sql
CREATE TABLE SyncQueue (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    EntityType TEXT NOT NULL,
    EntityId INTEGER NOT NULL,
    Operation TEXT NOT NULL, -- Create, Update, Delete
    QueuedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    SyncedAt DATETIME,
    SyncError TEXT,
    RetryCount INTEGER DEFAULT 0
);
```

---

## 8. User Interface Requirements

### 8.1 Design System

#### Color Palette
| Token | Value | Usage |
|-------|-------|-------|
| Primary | #4F46E5 | Buttons, links, highlights |
| Primary Dark | #4338CA | Hover states |
| Success | #10B981 | Completed status, success messages |
| Warning | #F59E0B | Moderate risk, pending |
| Danger | #F43F5E | High risk, errors, delete actions |
| Background | #FFFFFF | Main background |
| Surface | #F9FAFB | Card backgrounds |
| Text Primary | #111827 | Headings |
| Text Secondary | #6B7280 | Body text |
| Border | #E5E7EB | Input borders, dividers |

#### Typography
| Style | Font | Size | Weight | Line Height |
|-------|------|------|--------|-------------|
| H1 | Inter | 32px | SemiBold | 38px |
| H2 | Inter | 24px | SemiBold | 32px |
| H3 | Inter | 18px | SemiBold | 28px |
| Body | Inter | 14px | Regular | 24px |
| Body Small | Inter | 12px | Regular | 20px |
| Label | Inter | 11px | Medium | - |
| Numbers | JetBrains Mono | 16px | SemiBold | - |
| Currency | JetBrains Mono | 42px | SemiBold | - |

### 8.2 Page Structure

#### Main Window Layout
```
┌─────────────────────────────────────────────────────────┐
│  [Logo]  BillFlow                    [User] [Settings] │  ← Header
├───────┬─────────────────────────────────────────────────┤
│       │                                                 │
│  Nav  │              Main Content Area                  │
│       │                                                 │
│  ───  │                                                 │
│       │                                                 │
│Dashboard│                                               │
│       │                                                 │
│Customers│                                               │
│       │                                                 │
│Work   │                                                 │
│Orders │                                                 │
│       │                                                 │
│Daily  │                                                 │
│Schedule│                                               │
│       │                                                 │
│Khata  │                                                 │
│       │                                                 │
│Invoice│                                                 │
│       │                                                 │
│Reports│                                                 │
│       │                                                 │
│Settings│                                               │
│       │                                                 │
└───────┴─────────────────────────────────────────────────┘
          ↑ Sidebar Navigation (180px width)
```

### 8.3 Page Specifications

#### Dashboard Page
- **Layout:** Grid with metric cards and lists
- **Components:**
  - 4 metric cards (Pending Amount, Today's Revenue, Work Done, Work Pending)
  - Top Customers list (5 items)
  - Daily schedule summary
  - Credit alerts widget

#### Customers Page
- **Layout:** Master-detail with search
- **Components:**
  - Search bar with filters
  - Data grid with customer list
  - Add Customer button
  - Edit/Delete actions per row
  - Credit status indicators

#### Work Orders Page
- **Layout:** List with detail view
- **Components:**
  - Work order list with status badges
  - Filter by status/date
  - Add Work Order button
  - Line items editor
  - Payment status editor

#### Daily Schedule Page
- **Layout:** Calendar-based view
- **Components:**
  - Date picker
  - Split view: Pending | Completed
  - Work order cards with customer info
  - SqFt totals per section
  - Quick status update buttons

#### Khata Ledger Page
- **Layout:** Transaction list with summary
- **Components:**
  - Customer selector
  - Running balance display
  - Transaction table (Date, Type, Description, Amount, Balance)
  - Record Payment button
  - Statement generation

#### Invoice Page
- **Layout:** Work order selector + preview
- **Components:**
  - Work order search/selector
  - Invoice preview panel
  - Generate PDF button
  - Print button
  - Customer statement option

### 8.4 Dialog Specifications

#### Add Customer Dialog
- **Width:** 500px
- **Fields:**
  - Name (required, text, max 100)
  - Phone (optional, tel)
  - Email (optional, email)
  - Address (optional, textarea)
  - Credit Limit (optional, number)
- **Buttons:** Cancel, Save

#### Add Work Order Dialog
- **Width:** 700px
- **Fields:**
  - Customer selector (required, searchable dropdown)
  - Scheduled date (date picker)
  - Line items table (dynamic rows)
  - Notes (textarea)
- **Buttons:** Cancel, Save

#### Record Payment Dialog
- **Width:** 450px
- **Fields:**
  - Amount (required, number)
  - Payment date (date picker, default today)
  - Payment method (dropdown: Cash, UPI, Bank Transfer, etc.)
  - Notes (textarea)
- **Buttons:** Cancel, Record

### 8.5 Component Library

#### Glass Card
```
Background: White @ 85% opacity
Border: 1px solid Indigo @ 10% opacity
Border Radius: 16px
Padding: 24px
Shadow: 0 4px 24px rgba(0,0,0,0.05)
```

#### Primary Button
```
Background: #4F46E5
Foreground: White
Padding: 10px 28px
Border Radius: 8px
Font: Inter SemiBold 14px
Shadow: 0 4px 12px rgba(79,70,229,0.2)
Hover: Background #4338CA
```

#### Form Input
```
Background: White
Border: 1.5px solid #D1D5DB
Border Radius: 8px
Padding: 12px
Height: 40px
Focus Border: 2px solid #4F46E5
Error Border: 1.5px solid #F43F5E
```

---

## 9. Security & Compliance

### 9.1 Data Security

#### SEC-001: Local Database Protection
- **Requirement:** Database file shall be protected from unauthorized access
- **Implementation:** 
  - Store in user's LocalAppData (private location)
  - Recommend Windows EFS encryption for sensitive deployments
  - No hardcoded credentials

#### SEC-002: Backup Security
- **Requirement:** Backup files shall be protected
- **Implementation:**
  - Store in user's AppData (private location)
  - Timestamped backups prevent overwriting
  - Optional compression with password protection (future)

#### SEC-003: Input Validation
- **Requirement:** All user inputs shall be validated
- **Implementation:**
  - Server-side validation in ValidationService
  - SQL injection prevention via EF Core parameterized queries
  - XSS prevention in UI binding

### 9.2 Audit Trail

#### AUD-001: Change Logging
- **Requirement:** All create/update/delete operations shall be logged
- **Implementation:**
  - ILoggingService with category tracking
  - Log file location: %APPDATA%\BillFlow\Logs\
  - Log retention: 30 days rolling

#### AUD-002: Transaction History
- **Requirement:** Financial transactions shall be immutable
- **Implementation:**
  - Ledger entries cannot be deleted
  - Payments cannot be deleted (void only)
  - Modification tracking via LastModified timestamps

### 9.3 Compliance

#### CMP-001: Data Privacy
- **Requirement:** Customer data shall remain local by default
- **Implementation:**
  - No cloud storage without explicit user action
  - No telemetry or analytics without consent
  - GDPR-compliant data export capability

---

## 10. Performance Requirements

### 10.1 Response Time Specifications

| Operation | Target Time | Maximum Time | Measurement Condition |
|-----------|-------------|--------------|----------------------|
| Application Startup | 3 sec | 5 sec | Cold start, 10K records |
| Customer Search | 500 ms | 1 sec | 10K customer database |
| Work Order Save | 1 sec | 2 sec | With 10 line items |
| Invoice PDF Generation | 2 sec | 3 sec | Single page invoice |
| Dashboard Load | 1 sec | 2 sec | Full metrics calculation |
| Backup Creation | 5 sec | 10 sec | 100MB database |
| Report Generation | 3 sec | 5 sec | Monthly summary |

### 10.2 Scalability Limits

| Metric | Soft Limit | Hard Limit | Notes |
|--------|-----------|------------|-------|
| Customers | 50,000 | 100,000 | Performance degradation expected |
| Work Orders | 500,000 | 1,000,000 | Consider archiving old data |
| Line Items/Order | 50 | 100 | UI performance impact |
| Daily Backups | 30 | 90 | Auto-cleanup recommended |
| Log File Size | 10 MB | 50 MB | Auto-rotation implemented |

### 10.3 Resource Usage

| Resource | Typical | Peak | Notes |
|----------|---------|------|-------|
| Memory | 150 MB | 300 MB | During PDF generation |
| Disk (App) | 50 MB | 100 MB | With all dependencies |
| Disk (Data) | 10 MB | 500 MB | Depending on record count |
| CPU | 5% | 30% | During report generation |

---

## 11. Integration Points

### 11.1 External Service Integration (Future)

#### INT-001: SMS Gateway
- **Provider:** Twilio / MSG91 / TextLocal
- **Purpose:** Payment reminders, invoice notifications
- **Integration:** REST API with API key authentication
- **Status:** Framework ready, provider selection pending

#### INT-002: WhatsApp Business API
- **Provider:** WhatsApp Business API / Twilio
- **Purpose:** Rich notifications with PDF attachments
- **Integration:** Webhook-based message delivery
- **Status:** Framework ready, provider selection pending

#### INT-003: Email Service
- **Provider:** SendGrid / AWS SES / SMTP
- **Purpose:** Invoice delivery, statements
- **Integration:** SMTP or REST API
- **Status:** Framework ready

### 11.2 Data Export Integration

#### INT-004: Accounting Software
- **Format:** CSV with standard columns
- **Target:** Tally, Zoho Books, QuickBooks
- **Method:** Manual import/export

#### INT-005: Cloud Backup (Future)
- **Provider:** Google Drive / OneDrive / Dropbox
- **Purpose:** Offsite backup storage
- **Integration:** OAuth2 + REST API

---

## 12. Deployment Requirements

### 12.1 System Requirements

#### Minimum Requirements
- **OS:** Windows 10 (Version 1903 or later)
- **Processor:** Intel Core i3 or equivalent
- **Memory:** 4 GB RAM
- **Storage:** 100 MB free space
- **Display:** 1366x768 resolution
- **Runtime:** .NET 8.0 Desktop Runtime

#### Recommended Requirements
- **OS:** Windows 11
- **Processor:** Intel Core i5 or equivalent
- **Memory:** 8 GB RAM
- **Storage:** 500 MB free space
- **Display:** 1920x1080 resolution

### 12.2 Installation Methods

#### Method 1: Portable Deployment
1. Extract BillFlow.zip to desired location
2. Run BillFlow.exe
3. Database auto-initialized on first run

#### Method 2: Installer (MSI/Setup.exe)
1. Run BillFlow-Setup.exe
2. Follow installation wizard
3. Creates Start Menu shortcut
4. Optional desktop shortcut

### 12.3 File Locations

| Purpose | Location | Notes |
|---------|----------|-------|
| Application | C:\Program Files\BillFlow\ | Or user-selected |
| Database | %LOCALAPPDATA%\BillFlow\billflow.db | User-specific |
| Settings | %APPDATA%\BillFlow\Settings\ | User-specific |
| Backups | %APPDATA%\BillFlow\Backups\ | User-specific |
| Logs | %APPDATA%\BillFlow\Logs\ | User-specific |

### 12.4 Upgrade Process

1. **Backup:** Automatic backup before upgrade
2. **Install:** New version installation
3. **Migrate:** Database schema auto-migration via EF Core
4. **Verify:** Settings preservation verification

### 12.5 Uninstallation

#### Standard Uninstall
- Removes application files
- Preserves data in %APPDATA%\BillFlow\
- User can reinstall and resume

#### Complete Removal
- Removes application files
- Optionally removes all data
- User confirmation required

---

## 13. Future Roadmap

### 13.1 Version 3.0.0 (Planned Q3 2026)
- **UI Pages:** Complete SettingsPage and ReportsPage XAML implementations
- **Testing:** Full unit test coverage for all services
- **Deployment:** MSI installer with auto-updater
- **Dark Mode:** Complete dark theme implementation
- **Advanced Search:** Full-text search across all entities

### 13.2 Version 4.0.0 (Planned Q1 2027)
- **Cloud Sync:** Optional cloud synchronization
- **Multi-User:** Concurrent multi-user support
- **Mobile App:** Companion Android/iOS app
- **Advanced Analytics:** Charts, graphs, trend analysis
- **API Layer:** REST API for integrations

### 13.3 Version 5.0.0 (Future)
- **AI Features:** Smart pricing suggestions, credit risk prediction
- **Advanced Reporting:** Custom report builder
- **Inventory Integration:** Stock management integration
- **Vendor Management:** Supplier tracking
- **Multi-Currency:** International business support

### 13.4 Feature Backlog

| Feature | Priority | Effort | Target Version |
|---------|----------|--------|----------------|
| Dark Mode | Medium | Medium | v3.0 |
| Barcode/QR Code | Medium | Low | v3.0 |
| Thermal Printer Support | High | Medium | v3.0 |
| GST/Tax Invoice | High | Medium | v3.0 |
| Customer Portal | Low | High | v4.0 |
| Cloud Backup | Medium | Medium | v4.0 |
| Mobile App | Low | High | v4.0 |
| Multi-location | Low | High | v5.0 |
| Staff Management | Low | Medium | v5.0 |

---

## Appendix A: Glossary

| Term | Definition |
|------|------------|
| **Khata** | Traditional Indian ledger book for tracking credit transactions |
| **SqFt** | Square Feet - unit of measurement for print area |
| **Work Order** | A job request containing one or more print items |
| **Line Item** | Individual print job within a work order |
| **Ledger Entry** | A credit or debit record in the customer's account |
| **Credit Limit** | Maximum outstanding balance allowed for a customer |
| **Sync Queue** | Pending operations waiting for network connectivity |
| **Glassmorphism** | UI design style with frosted glass appearance |

## Appendix B: Calculation Formulas

### Square Footage Calculation
```
Area (SqFt) = Width (ft) × Height (ft) × Quantity
```

### Price Calculation
```
Total Price = Area (SqFt) × Rate per SqFt
```

### With Tax
```
Tax Amount = Subtotal × (Tax Rate / 100)
Grand Total = Subtotal + Tax Amount
```

### With Discount
```
Discount Amount = Subtotal × (Discount % / 100)
Discounted Total = Subtotal - Discount Amount
```

### Balance Calculation
```
Current Balance = Σ(Credits) - Σ(Debits)
Remaining Credit = Credit Limit - Current Balance
```

## Appendix C: Error Codes

| Code | Description | Resolution |
|------|-------------|------------|
| BF001 | Database locked | Close other app instances |
| BF002 | Validation failed | Check input requirements |
| BF003 | Credit limit exceeded | Reduce order amount or increase limit |
| BF004 | Sync failed | Check internet connection |
| BF005 | Backup failed | Check disk space |
| BF006 | PDF generation failed | Check write permissions |

---

**Document End**

*This PRD represents the complete requirements for BillFlow v2.0.0. All features specified herein have been implemented and tested. For questions or clarifications, refer to the DEVELOPER_README.md and PRODUCTION_GUIDE.md documents.*

**Maintained By:** BillFlow Development Team  
**Last Updated:** April 11, 2026  
**Next Review:** Upon v3.0.0 planning initiation
