namespace Invora.FieldOps.Api.Models;

public class Issue
{
    public int Id { get; set; }
    public int VisitId { get; set; }
    public Visit? Visit { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Open";
}
