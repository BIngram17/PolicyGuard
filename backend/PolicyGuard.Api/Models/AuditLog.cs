namespace PolicyGuard.Api.Models;

public class AuditLog
{
    public int Id { get; set; }

    public string Action { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public int? EntityId { get; set; }

    public string Summary { get; set; } = string.Empty;

    public string Details { get; set; } = string.Empty;

    public string PerformedBy { get; set; } = "System";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}