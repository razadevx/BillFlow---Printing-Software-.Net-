using Microsoft.EntityFrameworkCore;
using BillFlow.Database;
using BillFlow.Models;

namespace BillFlow.Services;

public interface IKhataService
{
    Task<List<KhataEntry>> GetEntriesByCustomerAsync(int customerId);
    Task<decimal> GetCurrentBalanceAsync(int customerId);
    Task AddCreditEntryAsync(int customerId, decimal amount, string description, int? workOrderId = null);
    Task AddDebitEntryAsync(int customerId, decimal amount, string description, int? workOrderId = null, bool updateLastPaymentDate = true);
    Task<List<CustomerCreditSummary>> GetAllCustomersCreditSummaryAsync();
    Task<int> GetDaysSinceLastPaymentAsync(int customerId);
    Task<decimal> GetTodayPaymentsTotalAsync();
}

public class CustomerCreditSummary
{
    public Customer Customer { get; set; } = null!;
    public decimal TotalCredit { get; set; }
    public decimal CreditLimit { get; set; }
    public DateTime? LastPaymentDate { get; set; }
    public int DaysSincePayment { get; set; }
    public CreditRiskLevel RiskLevel { get; set; }

    public string DaysSincePaymentDisplay => TotalCredit <= 0 ? "—" : DaysSincePayment.ToString();

    public string RiskEmoji => RiskLevel switch
    {
        CreditRiskLevel.Clear => "🟢",
        CreditRiskLevel.Moderate => "🟡",
        CreditRiskLevel.HighRisk => "🔴",
        _ => "⚪"
    };
}

public class KhataService : IKhataService
{
    private readonly BillFlowDbContext _context;
    private readonly ICalculationService _calculationService;

    public KhataService(BillFlowDbContext context, ICalculationService calculationService)
    {
        _context = context;
        _calculationService = calculationService;
    }

    public async Task<List<KhataEntry>> GetEntriesByCustomerAsync(int customerId)
    {
        return await _context.KhataEntries
            .Where(k => k.CustomerId == customerId)
            .OrderBy(k => k.TransactionDate)
            .ThenBy(k => k.Id)
            .ToListAsync();
    }

    public async Task<decimal> GetCurrentBalanceAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        return customer?.TotalCredit ?? 0;
    }

    public async Task AddCreditEntryAsync(int customerId, decimal amount, string description, int? workOrderId = null)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null) throw new ArgumentException("Customer not found");

        var balanceBefore = customer.TotalCredit;
        var newBalance = balanceBefore + amount;

        var entry = new KhataEntry
        {
            CustomerId = customerId,
            Type = TransactionType.Credit,
            Amount = amount,
            Balance = newBalance,
            Notes = description,
            WorkOrderId = workOrderId,
            TransactionDate = DateTime.Now
        };

        _context.KhataEntries.Add(entry);
        customer.TotalCredit = newBalance;

        await _context.SaveChangesAsync();
    }

    public async Task AddDebitEntryAsync(int customerId, decimal amount, string description, int? workOrderId = null, bool updateLastPaymentDate = true)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null) throw new ArgumentException("Customer not found");

        var balanceBefore = customer.TotalCredit;
        var newBalance = balanceBefore - amount;

        var entry = new KhataEntry
        {
            CustomerId = customerId,
            Type = TransactionType.Debit,
            Amount = amount,
            Balance = newBalance,
            Notes = description,
            WorkOrderId = workOrderId,
            TransactionDate = DateTime.Now
        };

        _context.KhataEntries.Add(entry);
        customer.TotalCredit = newBalance;
        if (updateLastPaymentDate)
            customer.LastPaymentDate = DateTime.Now;

        await _context.SaveChangesAsync();
    }

    public async Task<List<CustomerCreditSummary>> GetAllCustomersCreditSummaryAsync()
    {
        var customers = await _context.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();

        var result = new List<CustomerCreditSummary>();

        foreach (var customer in customers)
        {
            var daysSincePayment = customer.TotalCredit <= 0
                ? 0
                : await GetDaysSinceLastPaymentAsync(customer.Id);
            var riskLevel = _calculationService.GetCreditRisk(customer, customer.LastPaymentDate);

            result.Add(new CustomerCreditSummary
            {
                Customer = customer,
                TotalCredit = customer.TotalCredit,
                CreditLimit = customer.CreditLimit,
                LastPaymentDate = customer.LastPaymentDate,
                DaysSincePayment = daysSincePayment,
                RiskLevel = riskLevel
            });
        }

        return result.OrderByDescending(c => c.TotalCredit).ThenBy(c => c.Customer.Name).ToList();
    }

    public async Task<decimal> GetTodayPaymentsTotalAsync()
    {
        var today = DateTime.Today;
        return await _context.KhataEntries
            .Where(e => e.Type == TransactionType.Debit && e.TransactionDate.Date == today)
            .SumAsync(e => e.Amount);
    }

    public async Task<int> GetDaysSinceLastPaymentAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer?.LastPaymentDate == null) return 999; // High number if never paid

        return (DateTime.Today - customer.LastPaymentDate.Value).Days;
    }
}
