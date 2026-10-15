
using System.ComponentModel.DataAnnotations;

namespace Invora.FieldOps.Api.DTOs;

public class CreateIssueDto
{
    [Range(1, int.MaxValue)]
    public int VisitId { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = "Open";
}
