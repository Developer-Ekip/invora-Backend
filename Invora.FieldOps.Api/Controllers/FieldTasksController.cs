using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
using Invora.FieldOps.Api.Services;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]

public class FieldTasksController : ControllerBase
{
    private readonly IFieldTaskService _fieldTaskService;

    public FieldTasksController(IFieldTaskService fieldTaskService)
    {
        _fieldTaskService = fieldTaskService;
    }

    // GET: Get all tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FieldTask>>> GetAll()
    {
        var tasks = await _fieldTaskService.GetAllAsync();

        return Ok(tasks);
    }

    // GET: Get a task by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<FieldTask>> GetById(int id)
    {
        var task = await _fieldTaskService.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    // POST: Create a task linked to an existing visit
    [HttpPost]
    public async Task<ActionResult<FieldTask>> Create(CreateFieldTaskDto dto)
    {
        var task = await _fieldTaskService.CreateAsync(dto);

        if (task == null)
            return BadRequest("Visit does not exist.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task
        );
    }

    // PUT: Update an existing task
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateFieldTaskDto updatedTask)
    {
        var updated = await _fieldTaskService.UpdateAsync(id, updatedTask);

        if (!updated)
            return BadRequest("Task does not exist or Visit is invalid.");

        return NoContent();
    }

    // DELETE: Delete a task
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _fieldTaskService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
