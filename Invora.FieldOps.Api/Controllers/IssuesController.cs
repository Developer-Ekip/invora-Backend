using Invora.FieldOps.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly AppDbContext _context;

    public IssuesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Retrieve all issues
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Issue>>> GetAll()
    {
        var issues = await _context.Issues
            .AsNoTracking()
            .ToListAsync();

        return Ok(issues);
    }

    // GET: Retrieve an issue by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Issue>> GetById(int id)
    {
        var issue = await _context.Issues
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null)
            return NotFound();

        return Ok(issue);
    }


    // POST: Create an issue linked to an existing visit
    [HttpPost]
    public async Task<ActionResult<Issue>> Create(CreateIssueDto dto)
    {
        // Validate the related visit
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == dto.VisitId);

        if (!visitExists)
            return BadRequest("Visit does not exist.");

        var issue = new Issue
        {
            VisitId = dto.VisitId,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status
        };

        _context.Issues.Add(issue);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = issue.Id },
            issue
        );
    }


    // PUT: Update an existing issue
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateIssueDto updatedIssue)
    {
        var issue = await _context.Issues.FindAsync(id);

        if (issue == null)
            return NotFound();

        // Validate the related visit
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == updatedIssue.VisitId);

        if (!visitExists)
            return BadRequest("Visit does not exist.");

        issue.VisitId = updatedIssue.VisitId;
        issue.Title = updatedIssue.Title;
        issue.Description = updatedIssue.Description;
        issue.Status = updatedIssue.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    // DELETE: Remove an issue by ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var issue = await _context.Issues.FindAsync(id);

        if (issue == null)
            return NotFound();

        _context.Issues.Remove(issue);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
