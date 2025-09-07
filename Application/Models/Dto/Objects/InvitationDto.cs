using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class InvitationDto
{
    public Guid Id { get; set; }

    public Guid FromUserId { get; set; } 
    public Guid TargetUserId { get; set; }
    public Guid TargetClubId { get; set; }

    public DateTime DateCreated { get; set; }
    public DateTime? DateResponded { get; set; }

    public bool? Response { get; set; }

    public static InvitationDto FromDatabaseObject(InvitationDbo model)
    {
        return new InvitationDto
        {
            Id = model.Id,
            FromUserId = model.FromUserId,
            TargetUserId = model.TargetUserId,
            TargetClubId = model.TargetClubId,
            DateCreated = model.DateCreated,
            DateResponded = model.DateResponded,
            Response = model.Response
        };
    }
}