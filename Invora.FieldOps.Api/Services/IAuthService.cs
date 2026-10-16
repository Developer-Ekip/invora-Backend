using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(RegisterDto dto); //yeni kullanici olusturur
    Task<User?> LoginAsync(LoginDto dto); //kullanici adini ve sifresini kontrol eder
}
