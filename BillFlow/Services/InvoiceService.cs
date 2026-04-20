using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using BillFlow.Database;
using BillFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace BillFlow.Services;

public interface IInvoiceService
{
    Task<byte[]> GenerateInvoicePdfAsync(int workOrderId);
    Task<byte[]> GenerateCustomerStatementPdfAsync(int customerId);
    Task<string> GetNextInvoiceNumberAsync();
}

public class InvoiceService : IInvoiceService
{
    private readonly BillFlowDbContext _context;

    public InvoiceService(BillFlowDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(int workOrderId)
    {
        var order = await _context.WorkOrders
            .Include(w => w.Customer)
            .Include(w => w.LineItems)
            .FirstOrDefaultAsync(w => w.Id == workOrderId);

        if (order == null)
            throw new InvalidOperationException("Work order not found");

        var invoiceNo = await GetNextInvoiceNumberAsync();
        var settings = await _context.Settings.FirstOrDefaultAsync()
                       ?? new AppSettings();

        using var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        // Fonts — PdfSharp 6.x requires family + style separately
        var fontHeader = new XFont("Arial", 20, XFontStyleEx.Bold);
        var fontTitle = new XFont("Arial", 16, XFontStyleEx.Bold);
        var fontNormal = new XFont("Arial", 11);
        var fontBold = new XFont("Arial", 11, XFontStyleEx.Bold);
        var fontMono = new XFont("Courier New", 10);

        // Company Header
        gfx.DrawString(settings.BusinessName, fontHeader, XBrushes.Black, new XRect(40, 40, page.Width - 80, 30), XStringFormats.TopLeft);
        gfx.DrawString(settings.Address ?? "", fontNormal, XBrushes.Gray, new XRect(40, 70, page.Width - 80, 20), XStringFormats.TopLeft);
        gfx.DrawString($"Phone: {settings.Phone}", fontNormal, XBrushes.Gray, new XRect(40, 85, page.Width - 80, 20), XStringFormats.TopLeft);

        // Invoice Title
        gfx.DrawString("INVOICE", fontTitle, XBrushes.Black, new XRect(40, 120, page.Width - 80, 30), XStringFormats.TopLeft);
        
        // Invoice Details
        gfx.DrawString($"Invoice #: {invoiceNo}", fontNormal, XBrushes.Black, new XRect(page.Width - 200, 120, 160, 20), XStringFormats.TopLeft);
        gfx.DrawString($"Date: {order.OrderDate:yyyy-MM-dd}", fontNormal, XBrushes.Black, new XRect(page.Width - 200, 140, 160, 20), XStringFormats.TopLeft);

        // Customer Details
        gfx.DrawString("Bill To:", fontBold, XBrushes.Black, new XRect(40, 160, 200, 20), XStringFormats.TopLeft);
        gfx.DrawString(order.Customer.Name, fontNormal, XBrushes.Black, new XRect(40, 180, 300, 20), XStringFormats.TopLeft);
        gfx.DrawString($"Customer Code: {order.Customer.CustomerCode}", fontNormal, XBrushes.Black, new XRect(40, 195, 300, 20), XStringFormats.TopLeft);
        gfx.DrawString($"Phone: {order.Customer.Phone}", fontNormal, XBrushes.Black, new XRect(40, 210, 300, 20), XStringFormats.TopLeft);

        // Line Items Table Header
        int tableY = 260;
        int col1 = 40;   // Description
        int col2 = 220;  // Dimensions
        int col3 = 330;  // Qty
        int col4 = 380;  // Area
        int col5 = 450;  // Rate
        int col6 = 510;  // Total

        gfx.DrawRectangle(XBrushes.LightGray, new XRect(40, tableY, page.Width - 80, 25));
        gfx.DrawString("Description", fontBold, XBrushes.Black, new XRect(col1, tableY + 5, 170, 20), XStringFormats.TopLeft);
        gfx.DrawString("Dimensions", fontBold, XBrushes.Black, new XRect(col2, tableY + 5, 100, 20), XStringFormats.TopLeft);
        gfx.DrawString("Qty", fontBold, XBrushes.Black, new XRect(col3, tableY + 5, 40, 20), XStringFormats.TopLeft);
        gfx.DrawString("Area", fontBold, XBrushes.Black, new XRect(col4, tableY + 5, 60, 20), XStringFormats.TopLeft);
        gfx.DrawString("Rate", fontBold, XBrushes.Black, new XRect(col5, tableY + 5, 50, 20), XStringFormats.TopLeft);
        gfx.DrawString("Total", fontBold, XBrushes.Black, new XRect(col6, tableY + 5, 80, 20), XStringFormats.TopLeft);

        // Line Items
        int rowY = tableY + 30;
        foreach (var item in order.LineItems.OrderBy(li => li.Id))
        {
            gfx.DrawString(item.Description, fontMono, XBrushes.Black, new XRect(col1, rowY, 200, 20), XStringFormats.TopLeft);
            gfx.DrawString($"{item.Width} × {item.Height}", fontMono, XBrushes.Black, new XRect(col2, rowY, 100, 20), XStringFormats.TopLeft);
            gfx.DrawString(item.Quantity.ToString(), fontMono, XBrushes.Black, new XRect(col3, rowY, 40, 20), XStringFormats.TopLeft);
            gfx.DrawString($"{item.Area:F1}", fontMono, XBrushes.Black, new XRect(col4, rowY, 60, 20), XStringFormats.TopLeft);
            gfx.DrawString($"{settings.CurrencySymbol} {item.Rate:N0}", fontMono, XBrushes.Black, new XRect(col5, rowY, 50, 20), XStringFormats.TopLeft);
            gfx.DrawString($"{settings.CurrencySymbol} {item.Total:N2}", fontMono, XBrushes.Black, new XRect(col6, rowY, 90, 20), XStringFormats.TopLeft);
            rowY += 20;
        }

        // Totals
        int totalsY = rowY + 20;
        gfx.DrawLine(XPens.Gray, 40, totalsY, page.Width - 40, totalsY);
        
        gfx.DrawString($"Total Area:", fontBold, XBrushes.Black, new XRect(page.Width - 250, totalsY + 10, 100, 20), XStringFormats.TopLeft);
        gfx.DrawString($"{order.TotalArea:F1} sqft", fontMono, XBrushes.Black, new XRect(page.Width - 150, totalsY + 10, 100, 20), XStringFormats.TopLeft);
        
        gfx.DrawString($"Current Bill:", fontBold, XBrushes.Black, new XRect(page.Width - 250, totalsY + 35, 100, 20), XStringFormats.TopLeft);
        gfx.DrawString($"{settings.CurrencySymbol} {order.TotalAmount:N2}", fontBold, XBrushes.Black, new XRect(page.Width - 150, totalsY + 35, 100, 20), XStringFormats.TopLeft);

        if (order.PastBill > 0)
        {
            gfx.DrawString($"Past Bill:", fontNormal, XBrushes.Black, new XRect(page.Width - 250, totalsY + 55, 100, 20), XStringFormats.TopLeft);
            gfx.DrawString($"{settings.CurrencySymbol} {order.PastBill:N2}", fontMono, XBrushes.Black, new XRect(page.Width - 150, totalsY + 55, 100, 20), XStringFormats.TopLeft);
        }

        gfx.DrawString($"Grand Total:", fontBold, XBrushes.Black, new XRect(page.Width - 250, totalsY + 75, 100, 20), XStringFormats.TopLeft);
        gfx.DrawString($"{settings.CurrencySymbol} {order.GrandTotal:N2}", fontBold, XBrushes.Black, new XRect(page.Width - 150, totalsY + 75, 100, 20), XStringFormats.TopLeft);

        if (order.AmountPaid > 0)
        {
            gfx.DrawString($"Amount Paid:", fontNormal, XBrushes.Black, new XRect(page.Width - 250, totalsY + 95, 100, 20), XStringFormats.TopLeft);
            gfx.DrawString($"{settings.CurrencySymbol} {order.AmountPaid:N2}", fontMono, XBrushes.Black, new XRect(page.Width - 150, totalsY + 95, 100, 20), XStringFormats.TopLeft);
        }

        var balance = order.PendingAmount;
        if (balance > 0)
        {
            gfx.DrawString($"Pending:", fontBold, XBrushes.Black, new XRect(page.Width - 250, totalsY + 115, 100, 20), XStringFormats.TopLeft);
            gfx.DrawString($"{settings.CurrencySymbol} {balance:N2}", fontBold, XBrushes.Black, new XRect(page.Width - 150, totalsY + 115, 100, 20), XStringFormats.TopLeft);
        }

        // Footer
        gfx.DrawString(settings.InvoiceTerms, fontNormal, XBrushes.Gray, new XRect(40, page.Height - 60, page.Width - 80, 20), XStringFormats.TopLeft);

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateCustomerStatementPdfAsync(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.WorkOrders)
            .ThenInclude(w => w.LineItems)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        if (customer == null)
            throw new InvalidOperationException("Customer not found");

        var ledger = await _context.KhataEntries
            .Where(l => l.CustomerId == customerId)
            .OrderBy(l => l.TransactionDate)
            .ToListAsync();

        using var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        var fontHeader = new XFont("Arial", 18, XFontStyleEx.Bold);
        var fontTitle = new XFont("Arial", 14, XFontStyleEx.Bold);
        var fontNormal = new XFont("Arial", 10);
        var fontBold = new XFont("Arial", 10, XFontStyleEx.Bold);

        // Header
        gfx.DrawString($"Customer Statement", fontHeader, XBrushes.Black, new XRect(40, 40, page.Width - 80, 30), XStringFormats.TopLeft);
        gfx.DrawString($"Customer: {customer.Name}", fontTitle, XBrushes.Black, new XRect(40, 80, 400, 25), XStringFormats.TopLeft);
        var settings = await _context.Settings.FirstOrDefaultAsync() ?? new AppSettings();
        
        gfx.DrawString($"Current Balance: {settings.CurrencySymbol} {customer.TotalCredit:N2}", fontBold, XBrushes.Black, new XRect(page.Width - 200, 80, 160, 25), XStringFormats.TopLeft);

        // Ledger Table
        int tableY = 120;
        gfx.DrawRectangle(XBrushes.LightGray, new XRect(40, tableY, page.Width - 80, 20));
        gfx.DrawString("Date", fontBold, XBrushes.Black, new XRect(50, tableY + 3, 80, 20), XStringFormats.TopLeft);
        gfx.DrawString("Type", fontBold, XBrushes.Black, new XRect(140, tableY + 3, 60, 20), XStringFormats.TopLeft);
        gfx.DrawString("Description", fontBold, XBrushes.Black, new XRect(210, tableY + 3, 250, 20), XStringFormats.TopLeft);
        gfx.DrawString("Amount", fontBold, XBrushes.Black, new XRect(470, tableY + 3, 80, 20), XStringFormats.TopLeft);
        gfx.DrawString("Balance", fontBold, XBrushes.Black, new XRect(560, tableY + 3, 80, 20), XStringFormats.TopLeft);

        int rowY = tableY + 25;
        foreach (var entry in ledger)
        {
            if (rowY > page.Height - 60)
            {
                page = document.AddPage();
                gfx = XGraphics.FromPdfPage(page);
                rowY = 40;
            }

            gfx.DrawString(entry.TransactionDate.ToString("yyyy-MM-dd"), fontNormal, XBrushes.Black, new XRect(50, rowY, 80, 18), XStringFormats.TopLeft);
            gfx.DrawString(entry.Type.ToString(), fontNormal, XBrushes.Black, new XRect(140, rowY, 60, 18), XStringFormats.TopLeft);
            gfx.DrawString(entry.Notes ?? "", fontNormal, XBrushes.Black, new XRect(210, rowY, 250, 18), XStringFormats.TopLeft);
            gfx.DrawString($"{settings.CurrencySymbol} {entry.Amount:N2}", fontNormal, XBrushes.Black, new XRect(470, rowY, 80, 18), XStringFormats.TopLeft);
            gfx.DrawString($"{settings.CurrencySymbol} {entry.Balance:N2}", fontNormal, XBrushes.Black, new XRect(560, rowY, 80, 18), XStringFormats.TopLeft);
            
            rowY += 18;
        }

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        var settings = await _context.Settings.FirstOrDefaultAsync();
        if (settings == null)
        {
            settings = new AppSettings();
            _context.Settings.Add(settings);
            await _context.SaveChangesAsync();
        }
        settings.LastInvoiceNumber++;
        settings.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return $"{settings.InvoicePrefix}-{DateTime.Now:yyyy}-{settings.LastInvoiceNumber:D4}";
    }
}
