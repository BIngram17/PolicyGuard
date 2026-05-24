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
public class PolicyReviewsController : ControllerBase
{
    private readonly PolicyGuardDbContext _context;
    private readonly PolicyAnalyzerService _analyzerService;
    private readonly AuditLogService _auditLogService;

    public PolicyReviewsController(
        PolicyGuardDbContext context,
        PolicyAnalyzerService analyzerService,
        AuditLogService auditLogService)
    {
        _context = context;
        _analyzerService = analyzerService;
        _auditLogService = auditLogService;
    }

    [HttpPost("analyze")]
    [Authorize(Roles = "Admin,Reviewer")]
    public async Task<IActionResult> AnalyzePolicy([FromBody] AnalyzePolicyRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new { message = "A review title is required." });
        }

        if (string.IsNullOrWhiteSpace(request.DocumentText))
        {
            return BadRequest(new { message = "Document text is required." });
        }

        var checklist = await _context.ComplianceChecklists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == request.ComplianceChecklistId);

        if (checklist == null)
        {
            return NotFound(new { message = "Compliance checklist not found." });
        }

        var checklistItems = checklist.Items
            .OrderBy(i => i.Id)
            .ToList();

        var results = _analyzerService.AnalyzeDocument(request.DocumentText, checklistItems);
        var overallScore = _analyzerService.CalculateOverallScore(results, checklistItems);

        var review = new PolicyReview
        {
            Title = request.Title.Trim(),
            DocumentText = request.DocumentText,
            ComplianceChecklistId = checklist.Id,
            OverallScore = overallScore,
            Status = "Completed",
            CreatedAt = DateTime.UtcNow,
            Results = results
        };

        _context.PolicyReviews.Add(review);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(
            action: "POLICY_REVIEW_CREATED",
            entityType: "PolicyReview",
            entityId: review.Id,
            summary: $"Policy review created: {review.Title}",
            details: $"Checklist: {checklist.Name}; Overall Score: {review.OverallScore}%; Passed: {results.Count(r => r.ResultStatus == "Passed")}; Needs Review: {results.Count(r => r.ResultStatus == "Needs Review")}; Missing: {results.Count(r => r.ResultStatus == "Missing")}",
            performedBy: GetCurrentUserEmail()
        );

        var response = new
        {
            review.Id,
            review.Title,
            Checklist = checklist.Name,
            review.OverallScore,
            review.Status,
            review.CreatedAt,
            Summary = new
            {
                Passed = results.Count(r => r.ResultStatus == "Passed"),
                NeedsReview = results.Count(r => r.ResultStatus == "Needs Review"),
                Missing = results.Count(r => r.ResultStatus == "Missing"),
                TotalItems = results.Count
            },
            Results = results.Select(r =>
            {
                var item = checklistItems.First(i => i.Id == r.ChecklistItemId);

                return new
                {
                    r.Id,
                    item.Requirement,
                    item.Description,
                    r.ResultStatus,
                    r.MatchedText,
                    r.Recommendation,
                    r.Score,
                    item.Weight
                };
            })
        };

        return Ok(response);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Reviewer,Auditor")]
    public async Task<IActionResult> GetAllReviews()
    {
        var reviews = await _context.PolicyReviews
            .Include(r => r.ComplianceChecklist)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.Title,
                Checklist = r.ComplianceChecklist != null ? r.ComplianceChecklist.Name : "Unknown",
                r.OverallScore,
                r.Status,
                r.CreatedAt
            })
            .ToListAsync();

        return Ok(reviews);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Reviewer,Auditor")]
    public async Task<IActionResult> GetReviewById(int id)
    {
        var review = await _context.PolicyReviews
            .Include(r => r.ComplianceChecklist)
            .Include(r => r.Results)
                .ThenInclude(result => result.ChecklistItem)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            return NotFound(new { message = "Policy review not found." });
        }

        var response = new
        {
            review.Id,
            review.Title,
            Checklist = review.ComplianceChecklist != null ? review.ComplianceChecklist.Name : "Unknown",
            review.OverallScore,
            review.Status,
            review.CreatedAt,
            Summary = new
            {
                Passed = review.Results.Count(r => r.ResultStatus == "Passed"),
                NeedsReview = review.Results.Count(r => r.ResultStatus == "Needs Review"),
                Missing = review.Results.Count(r => r.ResultStatus == "Missing"),
                TotalItems = review.Results.Count
            },
            Results = review.Results
                .OrderBy(r => r.ChecklistItemId)
                .Select(r => new
                {
                    r.Id,
                    Requirement = r.ChecklistItem != null ? r.ChecklistItem.Requirement : "Unknown",
                    Description = r.ChecklistItem != null ? r.ChecklistItem.Description : "",
                    r.ResultStatus,
                    r.MatchedText,
                    r.Recommendation,
                    r.Score,
                    Weight = r.ChecklistItem != null ? r.ChecklistItem.Weight : 0
                })
        };

        return Ok(response);
    }

    [HttpPost("{id}/audit-report-export")]
    [Authorize(Roles = "Admin,Reviewer,Auditor")]
    public async Task<IActionResult> LogReportExport(int id)
    {
        var review = await _context.PolicyReviews
            .Include(r => r.ComplianceChecklist)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            return NotFound(new { message = "Policy review not found." });
        }

        await _auditLogService.LogAsync(
            action: "REPORT_EXPORTED",
            entityType: "PolicyReview",
            entityId: review.Id,
            summary: $"Compliance report exported: {review.Title}",
            details: $"Checklist: {review.ComplianceChecklist?.Name ?? "Unknown"}; Overall Score: {review.OverallScore}%",
            performedBy: GetCurrentUserEmail()
        );

        return Ok(new
        {
            message = "Report export logged successfully.",
            reviewId = review.Id
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var review = await _context.PolicyReviews
            .Include(r => r.Results)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (review == null)
        {
            return NotFound(new { message = "Policy review not found." });
        }

        var deletedTitle = review.Title;
        var deletedScore = review.OverallScore;

        _context.PolicyReviews.Remove(review);
        await _context.SaveChangesAsync();

        await _auditLogService.LogAsync(
            action: "POLICY_REVIEW_DELETED",
            entityType: "PolicyReview",
            entityId: id,
            summary: $"Policy review deleted: {deletedTitle}",
            details: $"Deleted review score: {deletedScore}%",
            performedBy: GetCurrentUserEmail()
        );

        return Ok(new
        {
            message = "Policy review deleted successfully.",
            deletedReviewId = id
        });
    }

    private string GetCurrentUserEmail()
    {
        return User.FindFirstValue(ClaimTypes.Email)
            ?? User.FindFirstValue("email")
            ?? "UnknownUser";
    }
}