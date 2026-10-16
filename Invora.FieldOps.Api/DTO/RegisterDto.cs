using System.ComponentModel.DataAnnotations;

namespace Invora.FieldOps.Api.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)] //zorunlu 3-50 karakter
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(8)] //zorunlu min 8 karakter
    public string Password { get; set; } = string.Empty;
}
