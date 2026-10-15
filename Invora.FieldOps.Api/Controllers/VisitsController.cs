
using Microsoft.AspNetCore.Mvc;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
using Invora.FieldOps.Api.Services;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitsController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    // GET: Get all visits
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Visit>>> GetAll()
    {
        var visits = await _visitService.GetAllAsync();

        return Ok(visits);
    }

    // GET: Get a visit by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Visit>> GetById(int id)
    {
        var visit = await _visitService.GetByIdAsync(id);

        if (visit == null)
            return NotFound();

        return Ok(visit);
    }

    // POST: Create a new visit
    [HttpPost]
    public async Task<ActionResult<Visit>> Create(CreateVisitDto dto)
    {
        var visit = await _visitService.CreateAsync(dto);

        if (visit == null)
            return BadRequest("Site does not exist.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = visit.Id },
            visit
        );
    }

    // PUT: Update an existing visit
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateVisitDto updatedVisit)
    {
        var updated = await _visitService.UpdateAsync(id, updatedVisit);

        if (!updated)
            return BadRequest("Visit does not exist or Site is invalid.");

        return NoContent();
    }

    // DELETE: Delete a visit
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _visitService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
