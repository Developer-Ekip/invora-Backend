
using Microsoft.AspNetCore.Mvc;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Services;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
private readonly IAuthService _authService; //kullanici bilgilerini kontrol eden servis
private readonly JwtTokenService _jwtTokenService; //token üreten servis

public AuthController(
    IAuthService authService,
    JwtTokenService jwtTokenService)
{
    _authService = authService;
    _jwtTokenService = jwtTokenService;
}


    // Register a new FieldUser
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var user = await _authService.RegisterAsync(dto);

        if (user == null)
            return Problem(statusCode: 409, title: "Username already exists.");

        return StatusCode(201, new
        {
            user.Id,
            user.Username,
            user.Role
        });
    }

    // Validate user credentials

    // Validate credentials and generate a JWT token
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _authService.LoginAsync(dto);

        if (user == null)
            return Problem(statusCode: 401, title: "Invalid username or password.");

        var token = _jwtTokenService.GenerateToken(user);

        return Ok(new
        {
            token,
            user.Id,
            user.Username,
            user.Role
        });
    }

}
