using Microsoft.EntityFrameworkCore;
using BillFlow.Models;

namespace BillFlow.Database;

public class BillFlowDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<LineItem> LineItems { get; set; }
    public DbSet<KhataEntry> KhataEntries { get; set; }
    public DbSet<DailyScheduleItem> DailyScheduleItems { get; set; }
    public DbSet<AppSettings> Settings { get; set; }

    public BillFlowDbContext(DbContextOptions<BillFlowDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customer indexes
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.CustomerCode)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Name);

        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Phone);

        // WorkOrder indexes
        modelBuilder.Entity<WorkOrder>()
            .HasIndex(w => w.OrderCode)
            .IsUnique();

        modelBuilder.Entity<WorkOrder>()
            .HasIndex(w => w.CustomerId);

        modelBuilder.Entity<WorkOrder>()
            .HasIndex(w => w.WorkStatus);

        modelBuilder.Entity<WorkOrder>()
            .HasIndex(w => w.ScheduledDate);

        modelBuilder.Entity<WorkOrder>()
            .HasIndex(w => w.PaymentStatus);

        // LineItem cascade delete
        modelBuilder.Entity<LineItem>()
            .HasOne(l => l.WorkOrder)
            .WithMany(w => w.LineItems)
            .HasForeignKey(l => l.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // KhataEntry indexes
        modelBuilder.Entity<KhataEntry>()
            .HasIndex(k => k.CustomerId);

        modelBuilder.Entity<KhataEntry>()
            .HasIndex(k => k.TransactionDate);

        modelBuilder.Entity<KhataEntry>()
            .HasOne(k => k.WorkOrder)
            .WithMany()
            .HasForeignKey(k => k.WorkOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        // DailyScheduleItem indexes
        modelBuilder.Entity<DailyScheduleItem>()
            .HasIndex(d => d.ScheduleDate);

        modelBuilder.Entity<DailyScheduleItem>()
            .HasIndex(d => d.WorkOrderId);

        // Seed default settings (single row)
        modelBuilder.Entity<AppSettings>().HasData(new AppSettings { Id = 1 });

        // Seed mock customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, CustomerCode = "C001", Name = "Ahmed Khan", Phone = "0300-1234567", Address = "Lahore, Pakistan", CreditLimit = 50000, TotalCredit = 15000, CreditRisk = CreditRiskLevel.Moderate, CreatedAt = DateTime.Now.AddDays(-30) },
            new Customer { Id = 2, CustomerCode = "C002", Name = "Fatima Ali", Phone = "0301-2345678", Address = "Karachi, Pakistan", CreditLimit = 30000, TotalCredit = 0, CreditRisk = CreditRiskLevel.Clear, CreatedAt = DateTime.Now.AddDays(-25) },
            new Customer { Id = 3, CustomerCode = "C003", Name = "Muhammad Hassan", Phone = "0302-3456789", Address = "Islamabad, Pakistan", CreditLimit = 75000, TotalCredit = 45000, CreditRisk = CreditRiskLevel.Moderate, CreatedAt = DateTime.Now.AddDays(-20) },
            new Customer { Id = 4, CustomerCode = "C004", Name = "Ayesha Siddiqui", Phone = "0303-4567890", Address = "Lahore, Pakistan", CreditLimit = 20000, TotalCredit = 8000, CreditRisk = CreditRiskLevel.Moderate, CreatedAt = DateTime.Now.AddDays(-15) },
            new Customer { Id = 5, CustomerCode = "C005", Name = "Bilal Ahmed", Phone = "0304-5678901", Address = "Rawalpindi, Pakistan", CreditLimit = 40000, TotalCredit = 0, CreditRisk = CreditRiskLevel.Clear, CreatedAt = DateTime.Now.AddDays(-10) },
            new Customer { Id = 6, CustomerCode = "C006", Name = "Sana Javed", Phone = "0305-6789012", Address = "Lahore, Pakistan", CreditLimit = 60000, TotalCredit = 25000, CreditRisk = CreditRiskLevel.Moderate, CreatedAt = DateTime.Now.AddDays(-5) },
            new Customer { Id = 7, CustomerCode = "C007", Name = "Imran Qureshi", Phone = "0306-7890123", Address = "Faisalabad, Pakistan", CreditLimit = 35000, TotalCredit = 12000, CreditRisk = CreditRiskLevel.Moderate, CreatedAt = DateTime.Now.AddDays(-3) },
            new Customer { Id = 8, CustomerCode = "C008", Name = "Nadia Khan", Phone = "0307-8901234", Address = "Multan, Pakistan", CreditLimit = 45000, TotalCredit = 0, CreditRisk = CreditRiskLevel.Clear, CreatedAt = DateTime.Now.AddDays(-2) },
            new Customer { Id = 9, CustomerCode = "C009", Name = "Zainab Ali", Phone = "0308-9012345", Address = "Lahore, Pakistan", CreditLimit = 25000, TotalCredit = 5000, CreditRisk = CreditRiskLevel.Moderate, CreatedAt = DateTime.Now.AddDays(-1) },
            new Customer { Id = 10, CustomerCode = "C010", Name = "Usman Ghani", Phone = "0309-0123456", Address = "Sialkot, Pakistan", CreditLimit = 55000, TotalCredit = 30000, CreditRisk = CreditRiskLevel.HighRisk, CreatedAt = DateTime.Now }
        );

        // Seed mock work orders
        modelBuilder.Entity<WorkOrder>().HasData(
            new WorkOrder { Id = 1, OrderCode = "WO-2024-001", CustomerId = 1, Notes = "Flex Printing - 4x8 Banner", WorkStatus = WorkStatus.Completed, PaymentStatus = PaymentStatus.Paid, TotalAmount = 5000, AmountPaid = 5000, PendingAmount = 0, GrandTotal = 5000, ScheduledDate = DateTime.Now.AddDays(-5), CreatedAt = DateTime.Now.AddDays(-10) },
            new WorkOrder { Id = 2, OrderCode = "WO-2024-002", CustomerId = 1, Notes = "Visiting Cards - 1000 pcs", WorkStatus = WorkStatus.InProgress, PaymentStatus = PaymentStatus.Credit, TotalAmount = 3000, AmountPaid = 0, PendingAmount = 3000, GrandTotal = 3000, ScheduledDate = DateTime.Now, CreatedAt = DateTime.Now.AddDays(-3) },
            new WorkOrder { Id = 3, OrderCode = "WO-2024-003", CustomerId = 3, Notes = "Brochure Design & Print", WorkStatus = WorkStatus.Received, PaymentStatus = PaymentStatus.Credit, TotalAmount = 15000, AmountPaid = 0, PendingAmount = 15000, GrandTotal = 15000, ScheduledDate = DateTime.Now.AddDays(2), CreatedAt = DateTime.Now.AddDays(-2) },
            new WorkOrder { Id = 4, OrderCode = "WO-2024-004", CustomerId = 4, Notes = "Sticker Printing", WorkStatus = WorkStatus.Completed, PaymentStatus = PaymentStatus.Credit, TotalAmount = 2000, AmountPaid = 0, PendingAmount = 2000, GrandTotal = 2000, ScheduledDate = DateTime.Now.AddDays(-3), CreatedAt = DateTime.Now.AddDays(-7) },
            new WorkOrder { Id = 5, OrderCode = "WO-2024-005", CustomerId = 6, Notes = "Standee Banner", WorkStatus = WorkStatus.InProgress, PaymentStatus = PaymentStatus.Credit, TotalAmount = 8000, AmountPaid = 0, PendingAmount = 8000, GrandTotal = 8000, ScheduledDate = DateTime.Now, CreatedAt = DateTime.Now.AddDays(-4) }
        );
    }
}
