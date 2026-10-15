using Invora.FieldOps.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VisitsController : ControllerBase
{
    private readonly AppDbContext _context;

    public VisitsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Visit>>> GetAll()
    {
        var visits = await _context.Visits
            .AsNoTracking()
            .ToListAsync();

        return Ok(visits);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Visit>> GetById(int id)
    {
        var visit = await _context.Visits
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        if (visit == null)
            return NotFound();

        return Ok(visit);
    }


[HttpPost]
public async Task<ActionResult<Visit>> Create(CreateVisitDto dto)
{
    // AnyAsync(): Ziyaretin bağlanacağı Site var mı?
    var siteExists = await _context.Sites
        .AnyAsync(s => s.Id == dto.SiteId);

    if (!siteExists)
        return BadRequest("Site does not exist.");

    // ID'yi veritabanı oluşturacak.
    var visit = new Visit
    {
        SiteId = dto.SiteId,
        VisitDate = dto.VisitDate,
        Status = dto.Status
    };

    // Yeni ziyareti kaydet.
    _context.Visits.Add(visit);
    await _context.SaveChangesAsync(); //PostgreSQL'e kaydediyor

    return CreatedAtAction(
        nameof(GetById),
        new { id = visit.Id },
        visit
    );
}



[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, UpdateVisitDto updatedVisit)
{
    var visit = await _context.Visits.FindAsync(id);

    if (visit == null)
        return NotFound();

    var siteExists = await _context.Sites
        .AnyAsync(s => s.Id == updatedVisit.SiteId);

    if (!siteExists)
        return BadRequest("Site does not exist.");

    visit.SiteId = updatedVisit.SiteId;
    visit.VisitDate = updatedVisit.VisitDate;
    visit.Status = updatedVisit.Status;

    await _context.SaveChangesAsync();

    return NoContent();
}


[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var visit = await _context.Visits.FindAsync(id);

    if (visit == null)
        return NotFound();

    _context.Visits.Remove(visit);

    await _context.SaveChangesAsync();

    return NoContent();
}


}
