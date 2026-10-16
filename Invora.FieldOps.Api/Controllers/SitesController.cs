
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class SitesController : ControllerBase
{
    private readonly AppDbContext _context;

    public SitesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/sites
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Site>>> GetAll()
    {
        var sites = await _context.Sites
            .AsNoTracking()
            .ToListAsync();

        return Ok(sites);
    }

    // GET: api/sites/1
    [HttpGet("{id}")]
                                         //Belirli bir lokasyonu getirir
    public async Task<ActionResult<Site>> GetById(int id) 
    {
        var site = await _context.Sites
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (site == null)
            return NotFound();

        return Ok(site);
    }
[Authorize(Roles = "Admin")]
    // POST: api/sites
    [HttpPost]
                                        //Yeni lokasyon oluşturur
    public async Task<ActionResult<Site>> Create(Site site)
    {
        var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == site.CompanyId);

        if (!companyExists)
            return BadRequest("Company does not exist.");

        site.Id = 0;
        site.Company = null!;

        _context.Sites.Add(site);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = site.Id },
            site
        );
    }
[Authorize(Roles = "Admin")]
    // PUT: api/sites/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Site updatedSite)
    {                               // lokasyon günceller
        var site = await _context.Sites.FindAsync(id);

        if (site == null)
            return NotFound();

        var companyExists = await _context.Companies
            .AnyAsync(c => c.Id == updatedSite.CompanyId);

        if (!companyExists)
            return BadRequest("Company does not exist.");

        site.Name = updatedSite.Name;
        site.CompanyId = updatedSite.CompanyId;

        await _context.SaveChangesAsync();

        return NoContent();
    }
[Authorize(Roles = "Admin")]
    // DELETE: api/sites/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var site = await _context.Sites.FindAsync(id);

        if (site == null)
            return NotFound();

        _context.Sites.Remove(site);
        await _context.SaveChangesAsync();

        return NoContent(); //http 204 yanitini olusturur
    }
}
