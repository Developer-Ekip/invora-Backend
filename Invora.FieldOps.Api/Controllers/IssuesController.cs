using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
using Invora.FieldOps.Api.Services;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class IssuesController : ControllerBase
{
    private readonly IIssueService _issueService;

    public IssuesController(IIssueService issueService)
    {
        _issueService = issueService;
    }

    // GET: Get all issues
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Issue>>> GetAll()
    {
        var issues = await _issueService.GetAllAsync();

        return Ok(issues);
    }

    // GET: Get an issue by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Issue>> GetById(int id)
    {
        var issue = await _issueService.GetByIdAsync(id);

        if (issue == null)
            return NotFound();

        return Ok(issue);
    }

    // POST: Create an issue linked to an existing visit
    [HttpPost]
    public async Task<ActionResult<Issue>> Create(CreateIssueDto dto)
    {
        var issue = await _issueService.CreateAsync(dto);

        if (issue == null)
            return BadRequest("Visit does not exist.");

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
        var updated = await _issueService.UpdateAsync(id, updatedIssue);

        if (!updated)
            return BadRequest("Issue does not exist or Visit is invalid.");

        return NoContent();
    }

    // DELETE: Delete an issue
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _issueService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
