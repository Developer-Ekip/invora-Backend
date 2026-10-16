using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
namespace Invora.FieldOps.Api.Services;
public class SiteService(AppDbContext context, CurrentUser user) : ISiteService
{
    private IQueryable<Site> Visible() => context.Sites.Where(x => user.IsAdmin || context.Visits.Any(v => v.SiteId == x.Id && v.AssignedUserId == user.Id));
    public async Task<List<Site>> GetAllAsync(string? search)
    {
        var query = Visible().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));
        return await query.OrderBy(x => x.Id).ToListAsync();
    }
    public Task<Site?> GetByIdAsync(int id) => Visible().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    public async Task<Site> CreateAsync(SaveSiteDto dto)
    {
        if (!await context.Companies.AnyAsync(c => c.Id == dto.CompanyId))
            throw new ArgumentException("Company does not exist.");
        var item = new Site { Name = dto.Name.Trim() };
        item.CompanyId = dto.CompanyId;
        context.Sites.Add(item);
        await context.SaveChangesAsync();
        return item;
    }
    public async Task<bool> UpdateAsync(int id, SaveSiteDto dto)
    {
        var item = await context.Sites.FindAsync(id);
        if (item == null) return false;
        if (!await context.Companies.AnyAsync(c => c.Id == dto.CompanyId))
            throw new ArgumentException("Company does not exist.");
        item.Name = dto.Name.Trim();
        item.CompanyId = dto.CompanyId;
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var item = await context.Sites.FindAsync(id);
        if (item == null) return false;
        context.Sites.Remove(item);
        await context.SaveChangesAsync();
        return true;
    }
}
