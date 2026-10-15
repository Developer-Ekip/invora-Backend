
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Controllers;

// Controllerin tanimlanmasi
[ApiController] // Enables API-specific behaviors
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly AppDbContext _context;

    // Dependency Injection
    public CompaniesController(AppDbContext context)
    {
        _context = context;
    }

    // Sirketleri listeleme
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Company>>> GetAll() //async ve await: islemleri asenkron yürütüyor
    {
        var companies = await _context.Companies
            .AsNoTracking()
            .ToListAsync();

        return Ok(companies);
    }

    // ID'ye gore sirket bulma
    [HttpGet("{id}")]
    public async Task<ActionResult<Company>> GetById(int id)
    {
        var company = await _context.Companies
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    // Yeni sirket olusturma
    [HttpPost]
    public async Task<ActionResult<Company>> Create(Company company)
    {
        company.Id = 0; // ID'yi PostgreSQL olusturacak
        company.Sites = new List<Site>();

        _context.Companies.Add(company);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = company.Id },
            company
        );
    }

    // Sirket guncelleme
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Company updatedCompany)
    {
        var company = await _context.Companies.FindAsync(id);

        if (company == null)
            return NotFound();

        company.Name = updatedCompany.Name;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Sirket silme
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var company = await _context.Companies.FindAsync(id);

        if (company == null)
            return NotFound();

        _context.Companies.Remove(company);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
