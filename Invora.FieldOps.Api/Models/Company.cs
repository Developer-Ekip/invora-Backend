namespace Invora.FieldOps.Api.Models;
public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
public List<Site> Sites { get; set; } = new();
}
