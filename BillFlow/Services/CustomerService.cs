using Microsoft.EntityFrameworkCore;
using BillFlow.Database;
using BillFlow.Models;

namespace BillFlow.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(int id);
    Task<Customer> CreateAsync(Customer customer);
    Task<Customer> UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
    Task<string> GenerateCustomerCodeAsync();
    Task<List<Customer>> SearchAsync(string searchTerm);
    Task<decimal> GetCurrentBalanceAsync(int customerId);
    Task UpdateCreditLimitAsync(int customerId, decimal newLimit);
    CreditRiskLevel GetCreditRiskLevel(Customer customer);
}

public class CustomerService : ICustomerService
{
    private readonly BillFlowDbContext _context;
    private readonly ICalculationService _calculationService;

    public CustomerService(BillFlowDbContext context, ICalculationService calculationService)
    {
        _context = context;
        _calculationService = calculationService;
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .Include(c => c.WorkOrders)
            .ThenInclude(w => w.LineItems)
            .Include(c => c.KhataEntries)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        // Validate
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ArgumentException("Customer name is required");

        customer.CustomerCode = await GenerateCustomerCodeAsync();
        customer.CreatedAt = DateTime.Now;
        customer.TotalCredit = 0;
        
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        
        return customer;
    }

    public async Task<Customer> UpdateAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.Name))
            throw new ArgumentException("Customer name is required");

        customer.UpdatedAt = DateTime.Now;
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
        
        return customer;
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            // Check if customer has work orders
            var hasWorkOrders = await _context.WorkOrders.AnyAsync(w => w.CustomerId == id);
            if (hasWorkOrders)
                throw new InvalidOperationException("Cannot delete customer with existing work orders");

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> GenerateCustomerCodeAsync()
    {
        var codes = await _context.Customers
            .Select(c => c.CustomerCode)
            .ToListAsync();

        var next = 1;
        foreach (var code in codes)
        {
            if (code.StartsWith("BF-", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(code.AsSpan(3), out var n))
                next = Math.Max(next, n + 1);
        }

        return $"BF-{next:D3}";
    }

    public async Task<List<Customer>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        searchTerm = searchTerm.ToLower();
        return await _context.Customers
            .Where(c => c.Name.ToLower().Contains(searchTerm) ||
                        c.CustomerCode.ToLower().Contains(searchTerm) ||
                        (c.Phone != null && c.Phone.Contains(searchTerm)))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<decimal> GetCurrentBalanceAsync(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        return customer?.TotalCredit ?? 0;
    }

    public async Task UpdateCreditLimitAsync(int customerId, decimal newLimit)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer != null)
        {
            customer.CreditLimit = newLimit;
            customer.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }
    }

    public CreditRiskLevel GetCreditRiskLevel(Customer customer)
    {
        return _calculationService.GetCreditRisk(customer, customer.LastPaymentDate);
    }
}
