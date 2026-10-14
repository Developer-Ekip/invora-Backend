
using Microsoft.AspNetCore.Mvc;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Controllers;

//controllerin tanimlanmasi
[ApiController] //enables api-specific behaviors
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private static readonly List<Company> Companies = new(); //reodonly: prevents reassigning the list reference
    private static int nextId = 1;

//sirketleri listeleme
    [HttpGet]
    public ActionResult<IEnumerable<Company>> GetAll()
    {
        return Ok(Companies);
    }

    [HttpGet("{id}")]
    public ActionResult<Company> GetById(int id)
    {
        var company = Companies.FirstOrDefault(c => c.Id == id);

        if (company == null)
            return NotFound();

        return Ok(company);
    }

    [HttpPost] //yeni sirket olusturma
    public ActionResult<Company> Create(Company company)
    {
        company.Id = nextId++;
        Companies.Add(company);

        return CreatedAtAction(
            nameof(GetById),
            new { id = company.Id },
            company
        );
    }

    [HttpPut("{id}")] //sirket güncelleme
    public IActionResult Update(int id, Company updatedCompany)
    {
        var company = Companies.FirstOrDefault(c => c.Id == id);

        if (company == null)
            return NotFound();

        company.Name = updatedCompany.Name;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var company = Companies.FirstOrDefault(c => c.Id == id);

        if (company == null)
            return NotFound();

        Companies.Remove(company);

        return NoContent();
    }
}
