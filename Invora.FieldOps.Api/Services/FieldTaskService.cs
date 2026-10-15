
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public class FieldTaskService : IFieldTaskService
{
    private readonly AppDbContext _context;

    public FieldTaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<FieldTask>> GetAllAsync()
    {
        return await _context.FieldTasks.ToListAsync();
    }

    public async Task<FieldTask?> GetByIdAsync(int id)
    {
        return await _context.FieldTasks.FindAsync(id);
    }

    public async Task<FieldTask?> CreateAsync(CreateFieldTaskDto dto)
    {
        // Check if the related visit exists
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == dto.VisitId);

        if (!visitExists)
            return null;

        var task = new FieldTask
        {
            VisitId = dto.VisitId,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status
        };

        _context.FieldTasks.Add(task);
        await _context.SaveChangesAsync();

        return task;
    }

    public async Task<bool> UpdateAsync(int id, UpdateFieldTaskDto dto)
    {
        var task = await _context.FieldTasks.FindAsync(id);

        if (task == null)
            return false;

        // Check if the related visit exists
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == dto.VisitId);

        if (!visitExists)
            return false;

        task.VisitId = dto.VisitId;
        task.Title = dto.Title;
        task.Description = dto.Description;
        task.Status = dto.Status;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.FieldTasks.FindAsync(id);

        if (task == null)
            return false;

        _context.FieldTasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }
}
