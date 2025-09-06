using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Dbo;

public class DiscussionDbo
{
    public Guid Id { get; set; }

    public Guid ActivityId { get; set; }
    public ActivityDbo Activity { get; set; } = null!;

    public Guid? UserId { get; set; }
    [MaxLength(128)] public string Username { get; set; } = null!;

    [MaxLength(256)] public string Title { get; set; } = null!;
    [MaxLength(4000)] public string? Description { get; set; } = null!;
    public bool IsClosed { get; set; } = false;
    public int? Page { get; set; }

    public ICollection<CommentDbo> Comments { get; set; } = [];

    public DateTime DateCreated { get; set; }
}