using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class DiscussionDto
{
    public Guid Id { get; set; }
    
    public ActivityDto Activity { get; set; } = null!;
    
    public Guid? UserId { get; set; }
    public string Username { get; set; } = null!;
    
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public bool IsClosed { get; set; }
    public int? Page { get; set; }
    
    public List<CommentDto> Comments { get; set; } = [];
    
    public DateTime DateCreated { get; set; }

    public static DiscussionDto FromDatabaseObject(DiscussionDbo model)
    {
        return new DiscussionDto
        {
            Id = model.Id,
            Activity = ActivityDto.FromDatabaseObject(model.Activity),
            UserId = model.UserId,
            Username = model.Username,
            Title = model.Title,
            Description = model.Description,
            IsClosed = model.IsClosed,
            Page = model.Page,
            Comments = model.Comments.Select(CommentDto.FromDatabaseObject).ToList(),
            DateCreated = model.DateCreated,
        };
    }
}