using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Services;
namespace Invora.FieldOps.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,FieldUser")]
public class CompaniesController(ICompanyService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search) => Ok((await service.GetAllAsync(search)).Select(x => x.ToDto()));
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await service.GetByIdAsync(id);
        return item == null ? NotFound() : Ok(item.ToDto());
    }
    [HttpPost, Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(SaveCompanyDto dto)
    {
        var item = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item.ToDto());
    }
    [HttpPut("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, SaveCompanyDto dto) => await service.UpdateAsync(id, dto) ? NoContent() : NotFound();
    [HttpDelete("{id:int}"), Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id) => await service.DeleteAsync(id) ? NoContent() : NotFound();
}
