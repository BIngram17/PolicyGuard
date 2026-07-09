using PolicyGuard.Api.Models;
using PolicyGuard.Api.Services;

namespace PolicyGuard.Api.Tests.Services;

public class PolicyAnalyzerServiceTests
{
    private readonly PolicyAnalyzerService _service = new();

    [Fact]
    public void AnalyzeDocument_WhenTwoKeywordsMatch_ReturnsPassedWithFullWeight()
    {
        var checklistItem = CreateChecklistItem(
            requirement: "Incident Response Plan",
            description: "Policy must define incident response ownership.",
            keywords: "incident,response,owner",
            weight: 20);

        var results = _service.AnalyzeDocument(
            "The INCIDENT response procedure assigns an owner for escalation.",
            new List<ChecklistItem> { checklistItem });

        var result = Assert.Single(results);
        Assert.Equal("Passed", result.ResultStatus);
        Assert.Equal(20.0, result.Score);
        Assert.Equal("incident, response, owner", result.MatchedText);
        AssertContainsIgnoringCase(result.Recommendation, "addressed");
    }

    [Fact]
    public void AnalyzeDocument_WhenOneKeywordMatches_ReturnsNeedsReviewWithHalfWeight()
    {
        var checklistItem = CreateChecklistItem(
            requirement: "Access Review",
            description: "Policy must describe periodic access review.",
            keywords: "access,review,quarterly",
            weight: 30);

        var results = _service.AnalyzeDocument(
            "Managers must complete access validation.",
            new List<ChecklistItem> { checklistItem });

        var result = Assert.Single(results);
        Assert.Equal("Needs Review", result.ResultStatus);
        Assert.Equal(15.0, result.Score);
        Assert.Equal("access", result.MatchedText);
        AssertContainsIgnoringCase(result.Recommendation, "may need more detail");
    }

    [Fact]
    public void AnalyzeDocument_WhenNoKeywordsMatch_ReturnsMissingWithZeroScore()
    {
        var checklistItem = CreateChecklistItem(
            requirement: "Data Retention",
            description: "Policy must define data retention rules.",
            keywords: "retention,archive,dispose",
            weight: 10);

        var results = _service.AnalyzeDocument(
            "The policy describes acceptable use only.",
            new List<ChecklistItem> { checklistItem });

        var result = Assert.Single(results);
        Assert.Equal("Missing", result.ResultStatus);
        Assert.Equal(0.0, result.Score);
        Assert.Equal(string.Empty, result.MatchedText);
        Assert.Contains("Data Retention", result.Recommendation);
        AssertContainsIgnoringCase(result.Recommendation, "data retention rules");
    }

    [Fact]
    public void AnalyzeDocument_WhenKeywordsHaveExtraSpaces_TrimsKeywordsBeforeMatching()
    {
        var checklistItem = CreateChecklistItem(
            requirement: "Security Training",
            description: "Policy must require training.",
            keywords: " training, awareness , annual ",
            weight: 10);

        var results = _service.AnalyzeDocument(
            "Annual security awareness training is required.",
            new List<ChecklistItem> { checklistItem });

        var result = Assert.Single(results);
        Assert.Equal("Passed", result.ResultStatus);
        Assert.Equal("training, awareness, annual", result.MatchedText);
    }

    [Fact]
    public void AnalyzeDocument_WhenMultipleChecklistItemsAreProvided_ReturnsResultForEachItem()
    {
        var checklistItems = new List<ChecklistItem>
        {
            CreateChecklistItem(1, "Encryption", "Requires encryption controls.", "encryption,keys", 25),
            CreateChecklistItem(2, "Vendor Risk", "Requires vendor risk review.", "vendor,risk", 25),
            CreateChecklistItem(3, "Backups", "Requires backup procedures.", "backup,recovery", 25)
        };

        var results = _service.AnalyzeDocument(
            "Encryption keys are rotated quarterly. Vendor contracts are reviewed.",
            checklistItems);

        Assert.Equal(3, results.Count);
        Assert.Contains(results, result => result.ChecklistItemId == 1 && result.ResultStatus == "Passed");
        Assert.Contains(results, result => result.ChecklistItemId == 2 && result.ResultStatus == "Needs Review");
        Assert.Contains(results, result => result.ChecklistItemId == 3 && result.ResultStatus == "Missing");
    }

    [Fact]
    public void CalculateOverallScore_WhenResultsAreWeighted_ReturnsWeightedPercentage()
    {
        var checklistItems = new List<ChecklistItem>
        {
            CreateChecklistItem(1, "Requirement A", "Description A", "alpha,beta", 40),
            CreateChecklistItem(2, "Requirement B", "Description B", "gamma,delta", 60)
        };

        var results = new List<ReviewResult>
        {
            new() { ChecklistItemId = 1, Score = 40 },
            new() { ChecklistItemId = 2, Score = 30 }
        };

        var score = _service.CalculateOverallScore(results, checklistItems);

        Assert.Equal(70.0, score);
    }

    [Fact]
    public void CalculateOverallScore_WhenTotalPossibleWeightIsZero_ReturnsZero()
    {
        var checklistItems = new List<ChecklistItem>
        {
            CreateChecklistItem(1, "Requirement A", "Description A", "alpha,beta", 0)
        };

        var results = new List<ReviewResult>
        {
            new() { ChecklistItemId = 1, Score = 10 }
        };

        var score = _service.CalculateOverallScore(results, checklistItems);

        Assert.Equal(0.0, score);
    }

    [Fact]
    public void CalculateOverallScore_RoundsToTwoDecimalPlaces()
    {
        var checklistItems = new List<ChecklistItem>
        {
            CreateChecklistItem(1, "Requirement A", "Description A", "alpha,beta", 3)
        };

        var results = new List<ReviewResult>
        {
            new() { ChecklistItemId = 1, Score = 2 }
        };

        var score = _service.CalculateOverallScore(results, checklistItems);

        Assert.Equal(66.67, score);
    }

    private static void AssertContainsIgnoringCase(string actual, string expectedSubstring)
    {
        // Keep the assertion compatible with the xUnit version used by the CI runner.
        // The BCL string comparison overload is stable across current .NET versions.
        Assert.True(
            actual.Contains(expectedSubstring, StringComparison.OrdinalIgnoreCase),
            $"Expected '{actual}' to contain '{expectedSubstring}' ignoring case.");
    }

    private static ChecklistItem CreateChecklistItem(
        string requirement,
        string description,
        string keywords,
        int weight)
    {
        return CreateChecklistItem(1, requirement, description, keywords, weight);
    }

    private static ChecklistItem CreateChecklistItem(
        int id,
        string requirement,
        string description,
        string keywords,
        int weight)
    {
        return new ChecklistItem
        {
            Id = id,
            Requirement = requirement,
            Description = description,
            Keywords = keywords,
            Weight = weight
        };
    }
}
