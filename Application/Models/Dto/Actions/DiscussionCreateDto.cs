namespace Application.Models.Dto.Actions;

public class DiscussionCreateDto
{
    public Guid ActivityId { get; set; }
    
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public int? Page { get; set; }
}