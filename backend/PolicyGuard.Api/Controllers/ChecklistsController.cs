using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolicyGuard.Api.Data;
using PolicyGuard.Api.DTOs;
using PolicyGuard.Api.Models;
using PolicyGuard.Api.Services;

namespace PolicyGuard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChecklistsController : ControllerBase
{
    private readonly PolicyGuardDbContext _context;
    private readonly AuditLogService _auditLogService;

    public ChecklistsController(
        PolicyGuardDbContext context,
        AuditLogService auditLogService)
    {
        _context = context;
        _auditLogService = auditLogService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Reviewer,Auditor")]
    public async Task<IActionResult> GetAllChecklists()
    {
        var checklists = await _context.ComplianceChecklists
            .Include(c => c.Items)
            .OrderBy(c => c.Name)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Category,
                c.CreatedAt,
                ItemCount = c.Items.Count
            })
            .ToListAsync();

        return Ok(checklists);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Reviewer,Auditor")]
    public async Task<IActionResult> GetChecklistById(int id)
    {
        var checklist = await _context.ComplianceChecklists
            .Include(c => c.Items)
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Category,
                c.CreatedAt,
                Items = c.Items
                    .OrderBy(i => i.Id)
                    .Select(i => new
                    {
                        i.Id,
                        i.Requirement,
                        i.Description,
                        i.Keywords,
                        i.Weight
                    })
            })
            .FirstOrDefaultAsync();

        if (checklist == null)
        {
            return NotFound(new { message = "Checklist not found." });
        }

        return Ok(checklist);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateChecklist([FromBody] CreateChecklistRequest request)
    {
        var validationError = ValidateChecklistRequest(
            request.Name,
            request.Description,
            request.Category,
            request.Items
        );

        if (validationError != null)
        {
            return BadRequest(new { message = validationError });
        }

        var checklist = new ComplianceChecklist
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Category = request.Category.Trim(),
            CreatedAt = DateTime.UtcNow,
            Items = request.Items.Select(item => new ChecklistItem
            {
                Requirement = item.Requirement.Trim(),
                Description = item.Description.Trim(),
                Keywords = item.Keywords.Trim(),
                Weight = item.Weight <= 0 ? 10 : item.Weight,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        _context.ComplianceChecklists.Add(checklist);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(
            action: "CHECKLIST_CREATED",
            entityType: "ComplianceChecklist",
            entityId: checklist.Id,
            summary: $"Checklist created: {checklist.Name}",
            details: $"Category: {checklist.Category}; Items: {checklist.Items.Count}",
            performedBy: GetCurrentUserEmail()
        );

        return CreatedAtAction(
            nameof(GetChecklistById),
            new { id = checklist.Id },
            new
            {
                checklist.Id,
                checklist.Name,
                checklist.Description,
                checklist.Category,
                checklist.CreatedAt,
                ItemCount = checklist.Items.Count
            }
        );
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateChecklist(
        int id,
        [FromBody] UpdateChecklistRequest request)
    {
        var checklist = await _context.ComplianceChecklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checklist == null)
        {
            return NotFound(new { message = "Checklist not found." });
        }

        var validationError = ValidateChecklistRequest(
            request.Name,
            request.Description,
            request.Category,
            request.Items
        );

        if (validationError != null)
        {
            return BadRequest(new { message = validationError });
        }

        var isChecklistUsedByReviews = await _context.PolicyReviews
            .AnyAsync(review => review.ComplianceChecklistId == id);

        if (isChecklistUsedByReviews)
        {
            return BadRequest(new
            {
                message = "This checklist has already been used in one or more policy reviews. To preserve review history, used checklists cannot be edited."
            });
        }

        checklist.Name = request.Name.Trim();
        checklist.Description = request.Description.Trim();
        checklist.Category = request.Category.Trim();

        _context.ChecklistItems.RemoveRange(checklist.Items);

        checklist.Items = request.Items.Select(item => new ChecklistItem
        {
            ComplianceChecklistId = checklist.Id,
            Requirement = item.Requirement.Trim(),
            Description = item.Description.Trim(),
            Keywords = item.Keywords.Trim(),
            Weight = item.Weight <= 0 ? 10 : item.Weight,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(
            action: "CHECKLIST_UPDATED",
            entityType: "ComplianceChecklist",
            entityId: checklist.Id,
            summary: $"Checklist updated: {checklist.Name}",
            details: $"Category: {checklist.Category}; Items: {checklist.Items.Count}",
            performedBy: GetCurrentUserEmail()
        );

        return Ok(new
        {
            checklist.Id,
            checklist.Name,
            checklist.Description,
            checklist.Category,
            ItemCount = checklist.Items.Count,
            message = "Checklist updated successfully."
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteChecklist(int id)
    {
        var checklist = await _context.ComplianceChecklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (checklist == null)
        {
            return NotFound(new { message = "Checklist not found." });
        }

        var isChecklistUsedByReviews = await _context.PolicyReviews
            .AnyAsync(review => review.ComplianceChecklistId == id);

        if (isChecklistUsedByReviews)
        {
            return BadRequest(new
            {
                message = "This checklist cannot be deleted because it has already been used in one or more policy reviews."
            });
        }

        var deletedName = checklist.Name;
        var deletedCategory = checklist.Category;
        var deletedItemCount = checklist.Items.Count;

        _context.ComplianceChecklists.Remove(checklist);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(
            action: "CHECKLIST_DELETED",
            entityType: "ComplianceChecklist",
            entityId: id,
            summary: $"Checklist deleted: {deletedName}",
            details: $"Category: {deletedCategory}; Items: {deletedItemCount}",
            performedBy: GetCurrentUserEmail()
        );

        return Ok(new
        {
            message = "Checklist deleted successfully.",
            deletedChecklistId = id
        });
    }

    private static string? ValidateChecklistRequest(
        string name,
        string description,
        string category,
        List<ChecklistItemRequest> items)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Checklist name is required.";
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return "Checklist description is required.";
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            return "Checklist category is required.";
        }

        if (items.Count == 0)
        {
            return "At least one checklist item is required.";
        }

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.Requirement))
            {
                return "Each checklist item must include a requirement.";
            }

            if (string.IsNullOrWhiteSpace(item.Description))
            {
                return "Each checklist item must include a description.";
            }

            if (string.IsNullOrWhiteSpace(item.Keywords))
            {
                return "Each checklist item must include keywords.";
            }
        }

        return null;
    }

    private string GetCurrentUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? "UnknownUser";
    }
}