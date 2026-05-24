namespace PolicyGuard.Api.DTOs;

public class ChecklistItemRequest
{
    public string Requirement { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Keywords { get; set; } = string.Empty;

    public int Weight { get; set; } = 10;
}