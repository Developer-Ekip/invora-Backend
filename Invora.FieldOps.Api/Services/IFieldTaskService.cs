
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public interface IFieldTaskService
{
    Task<List<FieldTask>> GetAllAsync();
    Task<FieldTask?> GetByIdAsync(int id);
    Task<FieldTask?> CreateAsync(CreateFieldTaskDto dto);
    Task<bool> UpdateAsync(int id, UpdateFieldTaskDto dto);
    Task<bool> DeleteAsync(int id);
}
