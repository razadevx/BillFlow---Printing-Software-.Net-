using System.ComponentModel.DataAnnotations;

namespace BillFlow.Models;

public enum ScheduleStatus
{
    Pending,
    Done
}

public class DailyScheduleItem
{
    [Key]
    public int Id { get; set; }

    public DateTime ScheduleDate { get; set; }

    [Required]
    public int WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;

    public int Priority { get; set; } = 0;

    public ScheduleStatus Status { get; set; } = ScheduleStatus.Pending;
}
