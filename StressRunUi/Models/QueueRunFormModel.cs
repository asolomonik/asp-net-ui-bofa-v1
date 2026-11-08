using System.ComponentModel.DataAnnotations;

namespace StressRunUi.Models;

public sealed class QueueRunFormModel
{
    [Required(ErrorMessage = "Scenario is required.")]
    [Display(Name = "Scenario Id")]
    public int? ScenarioId { get; set; }

    [Required(ErrorMessage = "Run date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Run Date")]
    public DateOnly? RunDate { get; set; }
}
