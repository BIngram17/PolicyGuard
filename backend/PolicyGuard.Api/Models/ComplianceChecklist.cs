namespace PolicyGuard.Api.Models;

public class ComplianceChecklist
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ChecklistItem> Items { get; set; } = new();
}