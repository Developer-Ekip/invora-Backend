
using System.ComponentModel.DataAnnotations;

namespace Invora.FieldOps.Api.DTOs;

public class UpdateVisitDto
{
    [Range(1, int.MaxValue)]
    public int SiteId { get; set; }

    [Required]
    public DateTime VisitDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;
}
