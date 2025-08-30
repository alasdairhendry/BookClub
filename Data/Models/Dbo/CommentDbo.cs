namespace Data.Models.Dbo;

public class CommentDbo
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    
    public Guid ActivitySectionId { get; set; }
    public ActivitySectionDbo? ActivitySection { get; set; } = null!;
    
    public ICollection<Guid> Children { get; set; } = null!;
}