using Microsoft.EntityFrameworkCore;
using Invora.FieldOps.Api.Data;
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
namespace Invora.FieldOps.Api.Services;
public class CompanyService(AppDbContext context, CurrentUser user) : ICompanyService
{
    private IQueryable<Company> Visible() => context.Companies.Where(x => user.IsAdmin || context.Visits.Any(v => v.Site!.CompanyId == x.Id && v.AssignedUserId == user.Id));
    public async Task<List<Company>> GetAllAsync(string? search)
    {
        var query = Visible().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search));
        return await query.OrderBy(x => x.Id).ToListAsync();
    }
    public Task<Company?> GetByIdAsync(int id) => Visible().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    public async Task<Company> CreateAsync(SaveCompanyDto dto)
    {
        var item = new Company { Name = dto.Name.Trim() };
        context.Companies.Add(item);
        await context.SaveChangesAsync();
        return item;
    }
    public async Task<bool> UpdateAsync(int id, SaveCompanyDto dto)
    {
        var item = await context.Companies.FindAsync(id);
        if (item == null) return false;
        item.Name = dto.Name.Trim();
        await context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var item = await context.Companies.FindAsync(id);
        if (item == null) return false;
        context.Companies.Remove(item);
        await context.SaveChangesAsync();
        return true;
    }
}
