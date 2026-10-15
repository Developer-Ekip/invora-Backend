
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public class VisitService : IVisitService
{
    private readonly AppDbContext _context;

    public VisitService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Visit>> GetAllAsync()
    {
        return await _context.Visits.ToListAsync();
    }

    public async Task<Visit?> GetByIdAsync(int id)
    {
        return await _context.Visits.FindAsync(id);
    }

    public async Task<Visit?> CreateAsync(CreateVisitDto dto)
    {
        // Check if the related site exists
        var siteExists = await _context.Sites
            .AnyAsync(s => s.Id == dto.SiteId);

        if (!siteExists)
            return null;

        var visit = new Visit
        {
            SiteId = dto.SiteId,
            VisitDate = dto.VisitDate,
            Status = dto.Status
        };

        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();

        return visit;
    }

    public async Task<bool> UpdateAsync(int id, UpdateVisitDto dto)
    {
        var visit = await _context.Visits.FindAsync(id);

        if (visit == null)
            return false;

        // Check if the related site exists
        var siteExists = await _context.Sites
            .AnyAsync(s => s.Id == dto.SiteId);

        if (!siteExists)
            return false;

        visit.SiteId = dto.SiteId;
        visit.VisitDate = dto.VisitDate;
        visit.Status = dto.Status;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var visit = await _context.Visits.FindAsync(id);

        if (visit == null)
            return false;

        _context.Visits.Remove(visit);
        await _context.SaveChangesAsync();

        return true;
    }
}
