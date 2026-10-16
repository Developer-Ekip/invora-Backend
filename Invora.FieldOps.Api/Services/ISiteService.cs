using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
namespace Invora.FieldOps.Api.Services;
public interface ISiteService
{
    Task<List<Site>> GetAllAsync(string? search);
    Task<Site?> GetByIdAsync(int id);
    Task<Site> CreateAsync(SaveSiteDto dto);
    Task<bool> UpdateAsync(int id, SaveSiteDto dto);
    Task<bool> DeleteAsync(int id);
}
