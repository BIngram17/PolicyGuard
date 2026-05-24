namespace PolicyGuard.Api.Models;

public class PolicyReview
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string DocumentText { get; set; } = string.Empty;

    public int ComplianceChecklistId { get; set; }

    public ComplianceChecklist? ComplianceChecklist { get; set; }

    public double OverallScore { get; set; }

    public string Status { get; set; } = "Completed";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ReviewResult> Results { get; set; } = new();
}