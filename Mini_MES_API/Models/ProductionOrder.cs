using System.ComponentModel.DataAnnotations;
using Mini_MES_API.Enums;

namespace Mini_MES_API.Models;

public class ProductionOrder
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string ProductSKU { get; set; }
    [Required] 
    public int Quantity { get; set; } = 0;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime PlannedEndTime { get; set; } 
    public int DefectCount { get; set; } 
    public double IdealCycleTimeMinutes { get; set; } 
    [Required]
    public Status Status { get; set; } = Status.Draft;
    public ICollection<WorkOrder> WorkOrders { get; set; }
}