namespace BlazorApp1.Models;

public class Customer
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public DateTime ImportedAt { get; set; }
}