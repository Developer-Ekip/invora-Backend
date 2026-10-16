using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;
namespace Invora.FieldOps.Api.Services;
public interface ICompanyService
{
    Task<List<Company>> GetAllAsync(string? search);
    Task<Company?> GetByIdAsync(int id);
    Task<Company> CreateAsync(SaveCompanyDto dto);
    Task<bool> UpdateAsync(int id, SaveCompanyDto dto);
    Task<bool> DeleteAsync(int id);
}
