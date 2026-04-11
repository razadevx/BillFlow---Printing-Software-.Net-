using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillFlow.Models;

public class LineItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;

    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "REAL")]
    public decimal Width { get; set; }

    [Column(TypeName = "REAL")]
    public decimal Height { get; set; }

    public int Quantity { get; set; } = 1;

    [Column(TypeName = "REAL")]
    public decimal Area { get; set; } // Auto: Width × Height × Quantity

    [Column(TypeName = "REAL")]
    public decimal Rate { get; set; } = 50; // Default PKR 50/SqFt

    [Column(TypeName = "REAL")]
    public decimal Total { get; set; } // Auto: Area × Rate
}
