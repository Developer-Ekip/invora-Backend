namespace Invora.FieldOps.Api.Models;

public class Visit
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public Site? Site { get; set; }

    public DateTime VisitDate { get; set; }

    public string Status { get; set; } = "Planned";
}