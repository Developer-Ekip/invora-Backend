
using System.ComponentModel.DataAnnotations;

namespace Invora.FieldOps.Api.DTOs;

public class UpdateVisitDto
{
    [Range(1, int.MaxValue)]
    public int SiteId { get; set; }

    [Range(1, int.MaxValue)]
    public int? AssignedUserId { get; set; }

    [Range(typeof(DateTime), "2000-01-01", "2100-01-01")]
    public DateTime VisitDate { get; set; }

    [Required]
    [RegularExpression("^(Planned|InProgress|Completed|Cancelled)$")]
    public string Status { get; set; } = string.Empty;
}
