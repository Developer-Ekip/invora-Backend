using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
using Invora.FieldOps.Api.Services;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,FieldUser")]

public class FieldTasksController : ControllerBase
{
    private readonly IFieldTaskService _fieldTaskService;

    public FieldTasksController(IFieldTaskService fieldTaskService)
    {
        _fieldTaskService = fieldTaskService;
    }

    // GET: Get all tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FieldTaskResponseDto>>> GetAll()
    {
        var tasks = await _fieldTaskService.GetAllAsync();

        return Ok(tasks.Select(x => x.ToDto()));
    }

    // GET: Get a task by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<FieldTaskResponseDto>> GetById(int id)
    {
        var task = await _fieldTaskService.GetByIdAsync(id);

        if (task == null)
            return NotFound();

        return Ok(task.ToDto());
    }

    // POST: Create a task linked to an existing visit
    [HttpPost]
    public async Task<ActionResult<FieldTaskResponseDto>> Create(CreateFieldTaskDto dto)
    {
        var task = await _fieldTaskService.CreateAsync(dto);

        if (task == null)
            return Problem(statusCode: 400, title: "Invalid request", detail: "Visit does not exist.");

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task.ToDto()
        );
    }

    // PUT: Update an existing task
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateFieldTaskDto updatedTask)
    {
        var updated = await _fieldTaskService.UpdateAsync(id, updatedTask);

        if (!updated)
            return Problem(statusCode: 400, title: "Invalid request", detail: "Task does not exist or Visit is invalid.");

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
