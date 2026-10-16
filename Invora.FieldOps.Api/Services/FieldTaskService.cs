
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public class FieldTaskService : IFieldTaskService
{
    private readonly AppDbContext _context;
    private readonly CurrentUser _user;

    public FieldTaskService(AppDbContext context, CurrentUser user)
    {
        _context = context;
        _user = user;
    }

    private IQueryable<FieldTask> Visible() => _context.FieldTasks
        .Where(x => _user.IsAdmin || x.Visit!.AssignedUserId == _user.Id);

    public async Task<List<FieldTask>> GetAllAsync()
    {
        return await Visible().AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<FieldTask?> GetByIdAsync(int id)
    {
        return await Visible().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<FieldTask?> CreateAsync(CreateFieldTaskDto dto)
    {
        // Check if the related visit exists
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == dto.VisitId && (_user.IsAdmin || v.AssignedUserId == _user.Id));

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
        var task = await Visible().FirstOrDefaultAsync(x => x.Id == id);

        if (task == null)
            return false;

        // Check if the related visit exists
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == dto.VisitId && (_user.IsAdmin || v.AssignedUserId == _user.Id));

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
        var task = await Visible().FirstOrDefaultAsync(x => x.Id == id);

        if (task == null)
            return false;

        _context.FieldTasks.Remove(task);
        await _context.SaveChangesAsync();

        return true;
    }
}
