namespace Invora.FieldOps.Api.Models;

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty; //sifrenin güvenli bir sekilde hashlenmis hali

    public string Role { get; set; } = "FieldUser";
}
