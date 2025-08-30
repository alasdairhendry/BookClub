namespace Data.Models.Dbo;

public class ActivitySectionDbo
{
    public Guid Id { get; set; }

    public Guid ActivityId { get; set; }
    public ActivityDbo? Activity { get; set; } = null!;
    
    public int SectionRangeMin {get; set; }
    public int SectionRangeMax {get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? TargetCompletionDate { get; set; }
    
    public DateTime? CompletionDate { get; set; }
    
    public ICollection<CommentDbo> Comments { get; set; } = null!;
}