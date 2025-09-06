using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Dbo;

public class CommentDbo
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }

    public Guid? UserId { get; set; }
    [MaxLength(128)] public string Username { get; set; } = null!;

    public Guid DiscussionId { get; set; }
    public DiscussionDbo Discussion { get; set; } = null!;

    [MaxLength(4000)]
    public string Message { get; set; } = null!;
    public bool SoftDelete { get; set; } = false;

    // public ICollection<Guid> Children { get; set; } = [];

    public DateTime DateCreated { get; set; }
}