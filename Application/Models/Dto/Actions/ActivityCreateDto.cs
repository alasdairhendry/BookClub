namespace Application.Models.Dto.Actions;

public class ActivityCreateDto
{
    public Guid RecordId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? TargetCompletionDate { get; set; }
}