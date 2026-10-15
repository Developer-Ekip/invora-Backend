using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Controllers;

[ApiController]

[Route("api/[controller]")]
public class FieldTasksController : ControllerBase
{
    // Database context used to access the database.
    private readonly AppDbContext _context;

    // Injects the database context using Dependency Injection.
    public FieldTasksController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/fieldtasks
    // Retrieves all tasks from the database.
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FieldTask>>> GetAll()
    {
        // AsNoTracking improves performance for read-only queries.
        var tasks = await _context.FieldTasks
            .AsNoTracking()
            .ToListAsync();

        return Ok(tasks);
    }

    // GET: api/fieldtasks/1
    // Retrieves a specific task by its ID.
    [HttpGet("{id}")]
    public async Task<ActionResult<FieldTask>> GetById(int id)
    {
        var task = await _context.FieldTasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        // HTTP 404 
        if (task == null)
            return NotFound();

        //HTTP 200
        return Ok(task);
    }

    // POST: api/fieldtasks
    // Creates a new task for an existing visit
    [HttpPost]
    public async Task<ActionResult<FieldTask>> Create(FieldTask task)
    {
        // Checks whether the related visit exists
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == task.VisitId);

        // Prevents creating tasks for non-existing visits
        if (!visitExists)
            return BadRequest("Visit does not exist.");

        // Ensures the database generates the task ID
        task.Id = 0;

        task.Visit = null;

        // Adds the new task to the database context.
        _context.FieldTasks.Add(task);

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task
        );
    }

    // PUT: api/fieldtasks/1
    // Updates an existing task
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, FieldTask updatedTask)
    {
        // Finds the task by its ID
        var task = await _context.FieldTasks.FindAsync(id);

        // Returns HTTP 404 if the task does not exist
        if (task == null)
            return NotFound();

        // Validates the related Visit before updating
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == updatedTask.VisitId);

        if (!visitExists)
            return BadRequest("Visit does not exist.");

        // Updates the task properties
        task.VisitId = updatedTask.VisitId;
        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.Status = updatedTask.Status;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/fieldtasks/1
    // Deletes an existing task by its ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        // Finds the task to delete
        var task = await _context.FieldTasks.FindAsync(id);

        if (task == null)
            return NotFound();

        // Marks the task for deletion
        _context.FieldTasks.Remove(task);

        // Permanently deletes the task from the database
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
