using Microsoft.EntityFrameworkCore;
using BillFlow.Database;
using BillFlow.Models;

namespace BillFlow.Services;

public interface IWorkOrderService
{
    Task<List<WorkOrder>> GetAllAsync();
    Task<WorkOrder?> GetByIdAsync(int id);
    Task<WorkOrder> CreateAsync(WorkOrder order, List<LineItem> lineItems);
    Task<WorkOrder> UpdateAsync(WorkOrder order);
    Task DeleteAsync(int id);
    Task<string> GenerateOrderCodeAsync();
    Task<List<WorkOrder>> GetByCustomerAsync(int customerId);
    Task<List<WorkOrder>> GetByStatusAsync(WorkStatus status);
    Task<List<WorkOrder>> GetByScheduledDateAsync(DateTime date);
    Task UpdateStatusAsync(int workOrderId, WorkStatus newStatus);
    Task RecordPaymentAsync(int workOrderId, decimal amount);
    Task RecalculateOrderAsync(WorkOrder order);
    Task UpdateScheduledDateAsync(int workOrderId, DateTime newDate);
}

public class WorkOrderService : IWorkOrderService
{
    private readonly BillFlowDbContext _context;
    private readonly ICalculationService _calculationService;
    private readonly ICustomerService _customerService;
    private readonly IKhataService _khataService;

    public WorkOrderService(
        BillFlowDbContext context,
        ICalculationService calculationService,
        ICustomerService customerService,
        IKhataService khataService)
    {
        _context = context;
        _calculationService = calculationService;
        _customerService = customerService;
        _khataService = khataService;
    }

    public async Task<List<WorkOrder>> GetAllAsync()
    {
        return await _context.WorkOrders
            .Include(w => w.Customer)
            .OrderByDescending(w => w.OrderDate)
            .ToListAsync();
    }

    public async Task<WorkOrder?> GetByIdAsync(int id)
    {
        return await _context.WorkOrders
            .Include(w => w.Customer)
            .Include(w => w.LineItems)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<WorkOrder> CreateAsync(WorkOrder order, List<LineItem> lineItems)
    {
        // Generate order code
        order.OrderCode = await GenerateOrderCodeAsync();
        order.OrderDate = DateTime.Now;

        // Calculate line items
        foreach (var item in lineItems)
        {
            _calculationService.RecalculateLineItem(item);
        }

        // Calculate totals
        order.TotalArea = lineItems.Sum(i => i.Area);
        order.TotalAmount = lineItems.Sum(i => i.Total);
        order.GrandTotal = order.TotalAmount + order.PastBill;
        order.PendingAmount = order.GrandTotal - order.AmountPaid;

        // Set line items
        order.LineItems = lineItems;

        _context.WorkOrders.Add(order);

        await _context.SaveChangesAsync();

        if (order.PendingAmount > 0 &&
            (order.PaymentStatus == PaymentStatus.Credit || order.PaymentStatus == PaymentStatus.Partial))
        {
            await _khataService.AddCreditEntryAsync(
                order.CustomerId,
                order.PendingAmount,
                $"Work order {order.OrderCode}",
                order.Id);
        }

        return order;
    }

    public async Task<WorkOrder> UpdateAsync(WorkOrder order)
    {
        _context.WorkOrders.Update(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task DeleteAsync(int id)
    {
        var order = await _context.WorkOrders
            .Include(w => w.LineItems)
            .FirstOrDefaultAsync(w => w.Id == id);
            
        if (order != null)
        {
            if (order.PendingAmount > 0 &&
                (order.PaymentStatus == PaymentStatus.Credit || order.PaymentStatus == PaymentStatus.Partial))
            {
                await _khataService.AddDebitEntryAsync(
                    order.CustomerId,
                    order.PendingAmount,
                    $"Reversal — deleted work order {order.OrderCode}",
                    order.Id,
                    updateLastPaymentDate: false);
            }

            _context.WorkOrders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> GenerateOrderCodeAsync()
    {
        var today = DateTime.Now;
        var count = await _context.WorkOrders
            .CountAsync(w => w.OrderDate.Date == today.Date);
        return $"WO{today:yyyyMMdd}-{(count + 1).ToString("D3")}";
    }

    public async Task<List<WorkOrder>> GetByCustomerAsync(int customerId)
    {
        return await _context.WorkOrders
            .Include(w => w.LineItems)
            .Where(w => w.CustomerId == customerId)
            .OrderByDescending(w => w.OrderDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetByStatusAsync(WorkStatus status)
    {
        return await _context.WorkOrders
            .Include(w => w.Customer)
            .Where(w => w.WorkStatus == status)
            .OrderByDescending(w => w.OrderDate)
            .ToListAsync();
    }

    public async Task<List<WorkOrder>> GetByScheduledDateAsync(DateTime date)
    {
        return await _context.WorkOrders
            .Include(w => w.Customer)
            .Include(w => w.LineItems)
            .Where(w => w.ScheduledDate.HasValue && w.ScheduledDate.Value.Date == date.Date)
            .OrderBy(w => w.Customer.Name)
            .ToListAsync();
    }

    public async Task UpdateStatusAsync(int workOrderId, WorkStatus newStatus)
    {
        var order = await _context.WorkOrders.FindAsync(workOrderId);
        if (order != null)
        {
            order.WorkStatus = newStatus;
            
            // Update dates based on status
            switch (newStatus)
            {
                case WorkStatus.Completed:
                    // order.CompletedDate = DateTime.Now;
                    break;
                case WorkStatus.Delivered:
                    // order.DeliveredDate = DateTime.Now;
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task RecordPaymentAsync(int workOrderId, decimal amount)
    {
        var order = await _context.WorkOrders.FindAsync(workOrderId);
        if (order != null)
        {
            order.AmountPaid += amount;
            order.PendingAmount = order.GrandTotal - order.AmountPaid;

            if (order.PendingAmount <= 0)
                order.PaymentStatus = PaymentStatus.Paid;
            else if (order.AmountPaid > 0)
                order.PaymentStatus = PaymentStatus.Partial;

            await _khataService.AddDebitEntryAsync(
                order.CustomerId,
                amount,
                $"Payment for work order {order.OrderCode}",
                order.Id);

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateScheduledDateAsync(int workOrderId, DateTime newDate)
    {
        var order = await _context.WorkOrders.FindAsync(workOrderId);
        if (order != null)
        {
            order.ScheduledDate = newDate;
            await _context.SaveChangesAsync();
        }
    }

    public async Task RecalculateOrderAsync(WorkOrder order)
    {
        var lineItems = await _context.LineItems
            .Where(li => li.WorkOrderId == order.Id)
            .ToListAsync();

        order.TotalArea = lineItems.Sum(li => li.Area);
        order.TotalAmount = lineItems.Sum(li => li.Total);
        order.GrandTotal = order.TotalAmount + order.PastBill;
        order.PendingAmount = order.GrandTotal - order.AmountPaid;

        await _context.SaveChangesAsync();
    }
}
