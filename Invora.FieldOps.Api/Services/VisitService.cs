
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public class VisitService : IVisitService
{
    private readonly AppDbContext _context;
    private readonly CurrentUser _user;

    public VisitService(AppDbContext context, CurrentUser user)
    {
        _context = context;
        _user = user;
    }

    private IQueryable<Visit> Visible() => _context.Visits
        .Where(x => _user.IsAdmin || x.AssignedUserId == _user.Id);

    public async Task<List<Visit>> GetAllAsync()
    {
        return await Visible().AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<Visit?> GetByIdAsync(int id)
    {
        return await Visible().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Visit?> CreateAsync(CreateVisitDto dto)
    {
        // Check if the related site exists
        var siteExists = await _context.Sites
            .AnyAsync(s => s.Id == dto.SiteId);

        if (!siteExists)
            return null;

        await ValidateAssignmentAsync(dto.AssignedUserId);
        var visit = new Visit
        {
            AssignedUserId = dto.AssignedUserId,
            SiteId = dto.SiteId,
            VisitDate = dto.VisitDate.ToUniversalTime(),
            Status = dto.Status
        };

        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();

        return visit;
    }

    public async Task<bool> UpdateAsync(int id, UpdateVisitDto dto)
    {
        var visit = await Visible().FirstOrDefaultAsync(x => x.Id == id);

        if (visit == null)
            return false;

        // Check if the related site exists
        var siteExists = await _context.Sites
            .AnyAsync(s => s.Id == dto.SiteId);

        if (!siteExists)
            return false;

        await ValidateAssignmentAsync(dto.AssignedUserId);
        visit.AssignedUserId = dto.AssignedUserId;
        visit.SiteId = dto.SiteId;
        visit.VisitDate = dto.VisitDate.ToUniversalTime();
        visit.Status = dto.Status;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var visit = await Visible().FirstOrDefaultAsync(x => x.Id == id);

        if (visit == null)
            return false;

        _context.Visits.Remove(visit);
        await _context.SaveChangesAsync();

        return true;
    }
    private async Task ValidateAssignmentAsync(int? userId)
    {
        if (userId.HasValue && !await _context.Users.AnyAsync(u => u.Id == userId && u.Role == "FieldUser"))
            throw new ArgumentException("Assigned user must be an existing FieldUser.");
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        var visit = await Visible().FirstOrDefaultAsync(v => v.Id == id);
        if (visit == null) return false;
        visit.Status = status;
        await _context.SaveChangesAsync();
        return true;
    }
}
