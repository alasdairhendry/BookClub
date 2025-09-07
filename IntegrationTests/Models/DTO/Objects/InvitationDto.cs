using Domain.Models.Dbo;

namespace IntegrationTests.Models.DTO.Objects;

public class InvitationDto
{
    public Guid Id { get; set; }

    public UserDto FromUser { get; set; } = null!;
    public UserDto TargetUser { get; set; } = null!;
    public ClubDto TargetClub { get; set; } = null!;

    public DateTime DateCreated { get; set; }
    public DateTime? DateResponded { get; set; }

    public bool? Response { get; set; }

    public static InvitationDto FromDatabaseObject(InvitationDbo model)
    {
        return new InvitationDto
        {
            Id = model.Id,
            FromUser = UserDto.FromDatabaseObject(model.FromUser!),
            TargetUser = UserDto.FromDatabaseObject(model.TargetUser!),
            TargetClub = ClubDto.FromDatabaseObject(model.TargetClub!),
            DateCreated = model.DateCreated,
            DateResponded = model.DateResponded,
            Response = model.Response
        };
    }
}