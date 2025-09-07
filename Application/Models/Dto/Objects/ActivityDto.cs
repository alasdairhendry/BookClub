using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class ActivityDto
{
    public Guid Id { get; set; }
    
    public ActivityState State { get; set; }

    public ClubDto Club { get; set; } = null!;
    public RecordDto Record { get; set; } = null!;

    public DateTime StartDate { get; set; }
    
    public DateTime? TargetEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    
    public List<DiscussionDto> Discussions { get; set; } = [];

    public static ActivityDto FromDatabaseObject(ActivityDbo model)
    {
        return new ActivityDto
        {
            Id = model.Id,
            State = model.State,
            Club = ClubDto.FromDatabaseObject(model.Club),
            Record = RecordDto.FromDatabaseObject(model.Record),
            StartDate = model.StartDate,
            TargetEndDate = model.TargetEndDate,
            ActualEndDate = model.ActualEndDate,
            Discussions = model.Discussions.Select(DiscussionDto.FromDatabaseObject).ToList(),
        };
    }
}