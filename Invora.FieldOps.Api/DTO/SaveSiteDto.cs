using System.ComponentModel.DataAnnotations;
namespace Invora.FieldOps.Api.DTOs;
public class SaveSiteDto
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int CompanyId { get; set; }
}
