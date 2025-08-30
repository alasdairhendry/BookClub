namespace Data.Models.Dbo;

public class ActivityDbo
{
    public Guid Id { get; set; }
    
    public Guid RecordId { get; set; }
    public RecordDbo? Record { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    
    public int SectionType { get; set; }
    public ICollection<ActivitySectionDbo> Sections { get; set; } = null!;
}