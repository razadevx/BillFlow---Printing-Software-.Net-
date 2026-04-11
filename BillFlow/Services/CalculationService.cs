using BillFlow.Models;

namespace BillFlow.Services;

public interface ICalculationService
{
    void RecalculateLineItem(LineItem item);
    void RecalculateWorkOrder(WorkOrder order);
    CreditRiskLevel GetCreditRisk(Customer customer, DateTime? lastPayment);
    decimal CalculateGrandTotal(decimal currentTotal, decimal pastBill);
    decimal CalculatePendingAmount(decimal grandTotal, decimal amountPaid);
}

public class CalculationService : ICalculationService
{
    public void RecalculateLineItem(LineItem item)
    {
        item.Area = Math.Round(item.Width * item.Height * item.Quantity, 1);
        item.Total = Math.Round(item.Area * item.Rate, 2);
    }

    public void RecalculateWorkOrder(WorkOrder order)
    {
        order.TotalArea = Math.Round(order.LineItems.Sum(i => i.Area), 1);
        order.TotalAmount = Math.Round(order.LineItems.Sum(i => i.Total), 2);
        order.GrandTotal = order.TotalAmount + order.PastBill;
        order.PendingAmount = order.GrandTotal - order.AmountPaid;
    }

    public CreditRiskLevel GetCreditRisk(Customer customer, DateTime? lastPayment)
    {
        // If no credit outstanding, clear
        if (customer.TotalCredit <= 0) 
            return CreditRiskLevel.Clear;

        // If never paid and has credit, high risk
        if (lastPayment == null) 
            return CreditRiskLevel.HighRisk;

        // Check if credit limit exceeded
        if (customer.TotalCredit > customer.CreditLimit)
            return CreditRiskLevel.HighRisk;

        var days = (DateTime.Today - lastPayment.Value).Days;
        
        if (days <= 7) 
            return CreditRiskLevel.Clear;
        if (days <= 30) 
            return CreditRiskLevel.Moderate;
        
        return CreditRiskLevel.HighRisk;
    }

    public decimal CalculateGrandTotal(decimal currentTotal, decimal pastBill)
    {
        return currentTotal + pastBill;
    }

    public decimal CalculatePendingAmount(decimal grandTotal, decimal amountPaid)
    {
        return grandTotal - amountPaid;
    }
}
