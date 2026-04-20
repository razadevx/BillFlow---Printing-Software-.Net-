using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillFlow.Models;

public class AppSettings
{
    [Key]
    public int Id { get; set; } = 1; // Single row

    [MaxLength(100)]
    public string? BusinessName { get; set; } = "My Printing Business";

    [MaxLength(100)]
    public string? OwnerName { get; set; } = "";

    [MaxLength(20)]
    public string? Phone { get; set; } = "";

    [MaxLength(200)]
    public string? Address { get; set; } = "";

    [Column(TypeName = "REAL")]
    public decimal RatePerSqFt { get; set; } = 50;

    [Column(TypeName = "REAL")]
    public decimal DefaultCreditLimit { get; set; } = 10000;

    [MaxLength(20)]
    public string? InvoicePrefix { get; set; } = "INV";

    public int LastInvoiceNumber { get; set; } = 0;

    [MaxLength(500)]
    public string? InvoiceTerms { get; set; } = "Payment due within 15 days. Thank you for your business!";

    [MaxLength(10)]
    public string? CurrencySymbol { get; set; } = "PKR";

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
