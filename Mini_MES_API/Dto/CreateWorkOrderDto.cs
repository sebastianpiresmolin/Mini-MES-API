using System.ComponentModel.DataAnnotations;
namespace Mini_MES_API.Dto;

public class CreateWorkOrderDto
{
    [Required]
    [StringLength(100)]
    public string StepName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Duration must be at least 1 minute.")]
    public int DurationInMinutes { get; set; }
}