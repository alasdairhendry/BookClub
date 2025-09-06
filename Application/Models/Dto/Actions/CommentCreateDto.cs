namespace Application.Models.Dto.Actions;

public class CommentCreateDto
{
    public Guid ParentId { get; set; }
    public Guid DiscussionId { get; set; }
    public string Message { get; set; } = null!;
}