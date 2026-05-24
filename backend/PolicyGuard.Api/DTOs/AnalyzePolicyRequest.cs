namespace PolicyGuard.Api.DTOs;

public class AnalyzePolicyRequest
{
    public string Title { get; set; } = string.Empty;

    public string DocumentText { get; set; } = string.Empty;

    public int ComplianceChecklistId { get; set; }
}