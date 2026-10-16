using System.ComponentModel.DataAnnotations;

namespace Invora.FieldOps.Api.DTOs;

public class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

/* kullanici giris yapmak istediginde
API a username ve password bilgilerini gönderir */
