using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolicyGuard.Api.Data;

namespace PolicyGuard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Auditor")]
public class AuditLogsController : ControllerBase
{
    private readonly PolicyGuardDbContext _context;

    public AuditLogsController(PolicyGuardDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] string? action,
        [FromQuery] string? entityType,
        [FromQuery] int take = 100)
    {
        if (take <= 0 || take > 500)
        {
            take = 100;
        }

        var query = _context.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(log => log.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(log => log.EntityType == entityType);
        }

        var logs = await query
            .OrderByDescending(log => log.CreatedAt)
            .Take(take)
            .Select(log => new
            {
                log.Id,
                log.Action,
                log.EntityType,
                log.EntityId,
                log.Summary,
                log.Details,
                log.PerformedBy,
                log.CreatedAt
            })
            .ToListAsync();

        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuditLogById(int id)
    {
        var log = await _context.AuditLogs
            .Where(auditLog => auditLog.Id == id)
            .Select(auditLog => new
            {
                auditLog.Id,
                auditLog.Action,
                auditLog.EntityType,
                auditLog.EntityId,
                auditLog.Summary,
                auditLog.Details,
                auditLog.PerformedBy,
                auditLog.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (log == null)
        {
            return NotFound(new { message = "Audit log not found." });
        }

        return Ok(log);
    }
}