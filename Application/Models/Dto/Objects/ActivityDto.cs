using System.Text.Json.Serialization;
using Domain.Enums;
using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class ActivityDto
{
    public Guid Id { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ActivityState State { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? TargetEndDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    public Guid ClubId { get; set; }
    public RecordDto Record { get; set; } = null!;
    public List<Guid> Discussions { get; set; } = [];

    public static ActivityDto FromDatabaseObject(ActivityDbo model)
    {
        return new ActivityDto
        {
            Id = model.Id,
            State = model.State,
            ClubId = model.Club.Id,
            Record = RecordDto.FromDatabaseObject(model.Record),
            StartDate = model.StartDate,
            TargetEndDate = model.TargetEndDate,
            ActualEndDate = model.ActualEndDate,
            Discussions = model.Discussions.Select(x=>x.Id).ToList()
        };
    }

    public static ActivityDto? FromDatabaseObjectNullable(ActivityDbo? model)
    {
        return model is null ? null : FromDatabaseObject(model);
    }
}