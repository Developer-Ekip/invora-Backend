
using Invora.FieldOps.Api.DTOs;
using Invora.FieldOps.Api.Models;

namespace Invora.FieldOps.Api.Services;

public interface IVisitService
{
    Task<List<Visit>> GetAllAsync();
    Task<Visit?> GetByIdAsync(int id);
    Task<Visit?> CreateAsync(CreateVisitDto dto);
    Task<bool> UpdateAsync(int id, UpdateVisitDto dto);
    Task<bool> UpdateStatusAsync(int id, string status);
    Task<bool> DeleteAsync(int id);
}
