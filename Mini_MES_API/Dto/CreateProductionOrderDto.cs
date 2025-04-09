using System.ComponentModel.DataAnnotations;
using Mini_MES_API.Enums;

namespace Mini_MES_API.Dto;

public class CreateProductionOrderDto
{
    [Required]
    public string ProductSKU { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    [Required]
    public Status Status { get; set; } = Status.Draft;
}