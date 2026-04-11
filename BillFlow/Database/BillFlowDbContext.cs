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
    }
}
