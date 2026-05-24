using PolicyGuard.Api.Models;

namespace PolicyGuard.Api.Services;

public class PolicyAnalyzerService
{
    public List<ReviewResult> AnalyzeDocument(string documentText, List<ChecklistItem> checklistItems)
    {
        var results = new List<ReviewResult>();

        foreach (var item in checklistItems)
        {
            var keywords = item.Keywords
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(k => k.ToLower())
                .ToList();

            var normalizedDocument = documentText.ToLower();

            var matchedKeywords = keywords
                .Where(keyword => normalizedDocument.Contains(keyword))
                .ToList();

            string status;
            double score;
            string matchedText;
            string recommendation;

            if (matchedKeywords.Count >= 2)
            {
                status = "Passed";
                score = item.Weight;
                matchedText = string.Join(", ", matchedKeywords);
                recommendation = "Requirement appears to be addressed in the document.";
            }
            else if (matchedKeywords.Count == 1)
            {
                status = "Needs Review";
                score = item.Weight * 0.5;
                matchedText = matchedKeywords.First();
                recommendation = $"The document mentions '{matchedKeywords.First()}', but this section may need more detail.";
            }
            else
            {
                status = "Missing";
                score = 0;
                matchedText = "";
                recommendation = $"Add a section addressing: {item.Requirement}. {item.Description}";
            }

            results.Add(new ReviewResult
            {
                ChecklistItemId = item.Id,
                ResultStatus = status,
                MatchedText = matchedText,
                Recommendation = recommendation,
                Score = score
            });
        }

        return results;
    }

    public double CalculateOverallScore(List<ReviewResult> results, List<ChecklistItem> checklistItems)
    {
        var totalPossible = checklistItems.Sum(item => item.Weight);

        if (totalPossible == 0)
        {
            return 0;
        }

        var earnedScore = results.Sum(result => result.Score);

        return Math.Round((earnedScore / totalPossible) * 100, 2);
    }
}