using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }

    public Guid? UserId { get; set; }
    public string Username { get; set; } = null!;

    public DiscussionDto Discussion { get; set; } = null!;
    
    public string Message { get; set; } = null!;
    public bool SoftDelete { get; set; } = false;
    
    // public List<Guid> Children { get; set; } = [];
    
    public static CommentDto FromDatabaseObject(CommentDbo model)
    {
        return new CommentDto
        {
            Id = model.Id,
            ParentId = model.ParentId,
            UserId = model.UserId,
            Username = model.Username,
            Discussion = DiscussionDto.FromDatabaseObject(model.Discussion),
            Message = model.Message,
            SoftDelete = model.SoftDelete
            // Children = model.Children.ToList()
        };
    }
}