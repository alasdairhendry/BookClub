using Domain.Interfaces.Dbo;

namespace Domain.Models.Dbo;

public class CommentDbo
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    
    public Guid UserId { get; set; }
    public ApplicationUserDbo? User { get; set; }
    
    public Guid ActivitySectionId { get; set; }
    public ClubActivitySectionDbo? ActivitySection { get; set; } = null!;

    public ICollection<Guid> Children { get; set; } = [];
}