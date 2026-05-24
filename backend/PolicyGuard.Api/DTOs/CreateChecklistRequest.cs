namespace PolicyGuard.Api.DTOs;

public class CreateChecklistRequest
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public List<ChecklistItemRequest> Items { get; set; } = new();
}