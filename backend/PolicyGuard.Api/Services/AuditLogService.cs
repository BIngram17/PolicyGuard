using PolicyGuard.Api.Data;
using PolicyGuard.Api.Models;

namespace PolicyGuard.Api.Services;

public class AuditLogService
{
    private readonly PolicyGuardDbContext _context;

    public AuditLogService(PolicyGuardDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(
        string action,
        string entityType,
        int? entityId,
        string summary,
        string details = "",
        string performedBy = "System")
    {
        var auditLog = new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Summary = summary,
            Details = details,
            PerformedBy = performedBy,
            CreatedAt = DateTime.UtcNow
        };

        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
    }
}