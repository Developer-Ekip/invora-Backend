using System.ComponentModel.DataAnnotations;
namespace Invora.FieldOps.Api.DTOs;
public class UpdateVisitStatusDto
{
    [Required, RegularExpression("^(Planned|InProgress|Completed|Cancelled)$")]
    public string Status { get; set; } = string.Empty;
}
