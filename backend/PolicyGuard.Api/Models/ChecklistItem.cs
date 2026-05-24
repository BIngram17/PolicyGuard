namespace PolicyGuard.Api.Models;

public class ChecklistItem
{
    public int Id { get; set; }

    public int ComplianceChecklistId { get; set; }

    public ComplianceChecklist? ComplianceChecklist { get; set; }

    public string Requirement { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;

    public int Weight { get; set; } = 10;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}