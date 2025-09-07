using Domain.Enums;
using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class ClubDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public string? Motto { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPrivate { get; set; } = true;

    public ActivityDto? CurrentActivity { get; set; } = null!;

    public List<Guid> MembershipIds { get; set; } = [];
    public int ActivityCount { get; set; }
    public int MembershipCount { get; set; }
    public int InvitationCount { get; set; }

    public static ClubDto FromDatabaseObject(ClubDbo model, bool includeObjects = true)
    {
        var currentActivity = ActivityDto.FromDatabaseObjectNullable(model.Activities.FirstOrDefault(x => x.State == ActivityState.Active));

        return new ClubDto
        {
            Id = model.Id,
            Name = model.Name,
            Motto = model.Motto,
            IsPrivate = model.IsPrivate,
            ImageUrl = model.ImageUrl,
            MembershipIds = model.Memberships.Select(x => x.Id).ToList(),
            CurrentActivity = includeObjects ? currentActivity : null!,
            ActivityCount = model.Activities.Count,
            MembershipCount = model.Memberships.Count,           
        };
    }
}