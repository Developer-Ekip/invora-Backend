using System.ComponentModel.DataAnnotations;
namespace Invora.FieldOps.Api.DTOs;
public class SaveCompanyDto
{
    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;
}
