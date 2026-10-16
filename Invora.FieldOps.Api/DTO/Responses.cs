using Invora.FieldOps.Api.Models;
namespace Invora.FieldOps.Api.DTOs;
public record CompanyResponseDto(int Id, string Name);
public record SiteResponseDto(int Id, string Name, int CompanyId);
public record VisitResponseDto(int Id, int SiteId, DateTime VisitDate, string Status, int? AssignedUserId);
public record FieldTaskResponseDto(int Id, int VisitId, string Title, string Description, string Status);
public record IssueResponseDto(int Id, int VisitId, string Title, string Description, string Status);
public static class Responses
{
    public static CompanyResponseDto ToDto(this Company x) => new(x.Id, x.Name);
    public static SiteResponseDto ToDto(this Site x) => new(x.Id, x.Name, x.CompanyId);
    public static VisitResponseDto ToDto(this Visit x) => new(x.Id, x.SiteId, x.VisitDate, x.Status, x.AssignedUserId);
    public static FieldTaskResponseDto ToDto(this FieldTask x) => new(x.Id, x.VisitId, x.Title, x.Description, x.Status);
    public static IssueResponseDto ToDto(this Issue x) => new(x.Id, x.VisitId, x.Title, x.Description, x.Status);
}
