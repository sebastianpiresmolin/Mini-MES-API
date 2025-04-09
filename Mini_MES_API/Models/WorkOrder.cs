using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_MES_API.Models;

public class WorkOrder
{
    [Required]
    public int Id { get; set; }
    [Required]
    [ForeignKey(nameof(ProductionOrder))]
    public int ProductionOrderId { get; set; }
    public ProductionOrder? ProductionOrder { get; set; }
    public string StepName { get; set; }
    [Required]
    public string Description { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
    public int DurationInMinutes { get; set; }
}