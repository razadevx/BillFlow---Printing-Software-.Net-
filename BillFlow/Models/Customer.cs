using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillFlow.Models;

public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string CustomerCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [Column(TypeName = "REAL")]
    public decimal CreditLimit { get; set; } = 10000;

    [Column(TypeName = "REAL")]
    public decimal TotalCredit { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastPaymentDate { get; set; }

    // Navigation properties
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<KhataEntry> KhataEntries { get; set; } = new List<KhataEntry>();
}

public enum CreditRiskLevel
{
    Clear,      // 🟢 No balance or paid within 7 days
    Moderate,   // 🟡 Balance pending, last payment < 30 days
    HighRisk    // 🔴 Balance pending > 30 days or credit limit exceeded
}
