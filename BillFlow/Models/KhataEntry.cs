using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillFlow.Models;

public enum TransactionType
{
    Credit,   // Adding to customer's bill (work done)
    Debit,    // Reducing customer's bill (payment received)
    Payment   // Explicit payment entry
}

public class KhataEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime TransactionDate { get; set; } = DateTime.Now;

    public TransactionType Type { get; set; }

    [Column(TypeName = "REAL")]
    public decimal Amount { get; set; }

    [Column(TypeName = "REAL")]
    public decimal Balance { get; set; } // Running balance after this entry

    public int? WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
