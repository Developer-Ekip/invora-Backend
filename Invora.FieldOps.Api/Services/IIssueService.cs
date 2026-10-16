
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public interface IIssueService
{
    Task<List<Issue>> GetAllAsync();
    Task<Issue?> GetByIdAsync(int id);
    Task<Issue?> CreateAsync(CreateIssueDto dto);
    Task<bool> UpdateAsync(int id, UpdateIssueDto dto);
    Task<bool> DeleteAsync(int id);
}
