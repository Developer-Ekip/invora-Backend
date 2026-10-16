
using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public class IssueService : IIssueService
{
    private readonly AppDbContext _context;
    private readonly CurrentUser _user;

    public IssueService(AppDbContext context, CurrentUser user)
    {
        _context = context;
        _user = user;
    }

    private IQueryable<Issue> Visible() => _context.Issues
        .Where(x => _user.IsAdmin || x.Visit!.AssignedUserId == _user.Id);

    public async Task<List<Issue>> GetAllAsync()
    {
        return await Visible().AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<Issue?> GetByIdAsync(int id)
    {
        return await Visible().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Issue?> CreateAsync(CreateIssueDto dto)
    {
        // Check if the related visit exists
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == dto.VisitId && (_user.IsAdmin || v.AssignedUserId == _user.Id));

        if (!visitExists)
            return null;

        var issue = new Issue
        {
            VisitId = dto.VisitId,
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status
        };

        _context.Issues.Add(issue);
        await _context.SaveChangesAsync();

        return issue;
    }

    public async Task<bool> UpdateAsync(int id, UpdateIssueDto dto)
    {
        var issue = await Visible().FirstOrDefaultAsync(x => x.Id == id);

        if (issue == null)
            return false;

        // Check if the related visit exists
        var visitExists = await _context.Visits
            .AnyAsync(v => v.Id == dto.VisitId && (_user.IsAdmin || v.AssignedUserId == _user.Id));

        if (!visitExists)
            return false;

        issue.VisitId = dto.VisitId;
        issue.Title = dto.Title;
        issue.Description = dto.Description;
        issue.Status = dto.Status;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var issue = await Visible().FirstOrDefaultAsync(x => x.Id == id);

        if (issue == null)
            return false;

        _context.Issues.Remove(issue);
        await _context.SaveChangesAsync();

        return true;
    }
}
