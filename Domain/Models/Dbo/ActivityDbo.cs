using Domain.Enums;

namespace Domain.Models.Dbo;

public class ActivityDbo
{
    public Guid Id { get; set; }

    public ActivityState State { get; set; }

    public Guid ClubId { get; set; }
    public ClubDbo Club { get; set; } = null!;

    public Guid RecordId { get; set; }
    public RecordDbo Record { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? TargetEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    public ICollection<DiscussionDbo> Discussions { get; set; } = [];
}