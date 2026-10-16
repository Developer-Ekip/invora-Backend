using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
namespace Invora.FieldOps.Api.Controllers;
[ApiController, Route("api/users"), Authorize(Roles = "Admin")]
public class UsersController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await context.Users.AsNoTracking()
        .Where(u => u.Role == "FieldUser").OrderBy(u => u.Id)
        .Select(u => new { u.Id, u.Username, u.Role }).ToListAsync());
}
