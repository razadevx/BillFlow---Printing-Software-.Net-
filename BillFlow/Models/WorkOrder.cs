using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillFlow.Models;

public enum WorkStatus
{
    Received,
    InProgress,
    Ready,
    Delivered,
    Completed
}

public enum PaymentStatus
{
    Cash,
    Credit,
    Partial,
    Paid
}

public class WorkOrder
{
    [Key]
    public int Id { get; set; }

    public string? OrderCode { get; set; }

    [Required]
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime? ScheduledDate { get; set; }

    public WorkStatus WorkStatus { get; set; } = WorkStatus.Received;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Credit;

    [Column(TypeName = "REAL")]
    public decimal TotalArea { get; set; }

    [Column(TypeName = "REAL")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "REAL")]
    public decimal PastBill { get; set; }

    [Column(TypeName = "REAL")]
    public decimal GrandTotal { get; set; }

    [Column(TypeName = "REAL")]
    public decimal AmountPaid { get; set; }

    [Column(TypeName = "REAL")]
    public decimal PendingAmount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation properties
    public ICollection<LineItem> LineItems { get; set; } = new List<LineItem>();
}
