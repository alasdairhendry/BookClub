namespace Data.Models.Dbo;

public class ClubActivityDbo
{
    public Guid Id { get; set; }
    
    public Guid ClubId { get; set; }
    public ClubDbo? Club { get; set; } = null!;
    
    public Guid RecordId { get; set; }
    public RecordDbo? Record { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    
    public int SectionType { get; set; }
    public ICollection<ClubActivitySectionDbo> Sections { get; set; } = [];
}