using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
using Invora.FieldOps.Api.Services;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,FieldUser")]

public class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitsController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    // GET: Get all visits
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VisitResponseDto>>> GetAll()
    {
        var visits = await _visitService.GetAllAsync();

        return Ok(visits.Select(x => x.ToDto()));
    }

    // GET: Get a visit by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<VisitResponseDto>> GetById(int id)
    {
        var visit = await _visitService.GetByIdAsync(id);

        if (visit == null)
            return NotFound();

        return Ok(visit.ToDto());
    }

    // POST: Create a new visit
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<VisitResponseDto>> Create(CreateVisitDto dto)
    {
        var visit = await _visitService.CreateAsync(dto);

        if (visit == null)
            return Problem(statusCode: 400, title: "Invalid request", detail: "Site does not exist.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = visit.Id },
            visit.ToDto()
        );
    }

    // PUT: Update an existing visit
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateVisitDto updatedVisit)
    {
        var updated = await _visitService.UpdateAsync(id, updatedVisit);

        if (!updated)
            return Problem(statusCode: 400, title: "Invalid request", detail: "Visit does not exist or Site is invalid.");

        return NoContent();
    }

    // DELETE: Delete a visit
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _visitService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateVisitStatusDto dto)
    {
        if (!await _visitService.UpdateStatusAsync(id, dto.Status)) return NotFound();
        return NoContent();
    }
}
