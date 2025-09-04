using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class ClubMembershipDto
{
    public Guid Id { get; set; }

    public UserDto User { get; set; } = null!;
    // public ClubDto Club { get; set; } = null!;
    
    public DateTime MemberSince { get; set; }
    public bool IsAdmin { get; set; } = false;
    
    public static ClubMembershipDto FromDatabaseObject(ClubMembershipDbo model)
    {
        return new ClubMembershipDto
        {
            Id = model.Id,
            User = UserDto.FromDatabaseObject(model.User!),
            MemberSince = model.MemberSince,
            IsAdmin = model.IsAdmin
        };
    }
}