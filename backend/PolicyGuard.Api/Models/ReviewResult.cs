namespace PolicyGuard.Api.Models;

public class ReviewResult
{
    public int Id { get; set; }

    public int PolicyReviewId { get; set; }

    public PolicyReview? PolicyReview { get; set; }

    public int ChecklistItemId { get; set; }

    public ChecklistItem? ChecklistItem { get; set; }

    public string ResultStatus { get; set; } = string.Empty;

    public string MatchedText { get; set; } = string.Empty;

    public string Recommendation { get; set; } = string.Empty;

    public double Score { get; set; }
}