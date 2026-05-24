using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolicyGuard.Api.Data;

namespace PolicyGuard.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Reviewer,Auditor")]
public class DashboardController : ControllerBase
{
    private readonly PolicyGuardDbContext _context;

    public DashboardController(PolicyGuardDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetDashboardSummary()
    {
        var totalReviews = await _context.PolicyReviews.CountAsync();

        var averageScore = totalReviews > 0
            ? await _context.PolicyReviews.AverageAsync(r => r.OverallScore)
            : 0;

        var reviewsNeedingAttention = await _context.PolicyReviews
            .CountAsync(r => r.OverallScore < 70);

        var totalChecklists = await _context.ComplianceChecklists.CountAsync();

        var recentReviews = await _context.PolicyReviews
            .Include(r => r.ComplianceChecklist)
            .OrderByDescending(r => r.CreatedAt)
            .Take(5)
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

        var mostCommonMissingRequirement = await _context.ReviewResults
            .Include(r => r.ChecklistItem)
            .Where(r => r.ResultStatus == "Missing")
            .GroupBy(r => r.ChecklistItem != null ? r.ChecklistItem.Requirement : "Unknown")
            .Select(g => new
            {
                Requirement = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(g => g.Count)
            .FirstOrDefaultAsync();

        var summary = new
        {
            TotalReviews = totalReviews,
            AverageScore = Math.Round(averageScore, 2),
            ReviewsNeedingAttention = reviewsNeedingAttention,
            TotalChecklists = totalChecklists,
            MostCommonMissingRequirement = mostCommonMissingRequirement,
            RecentReviews = recentReviews
        };

        return Ok(summary);
    }
}