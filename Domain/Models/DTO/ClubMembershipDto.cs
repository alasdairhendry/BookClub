using Data.Models.Dbo;

namespace Domain.Models.DTO;

public class ClubMembershipDto
{
    public Guid Id { get; set; }

    public UserDto User { get; set; } = null!;
    public ClubDto Club { get; set; } = null!;
    
    public bool IsAdmin { get; set; } = false;
    
    public static ClubMembershipDto FromDatabaseObject(ClubMembershipDbo model)
    {
        return new ClubMembershipDto
        {
            Id = model.Id,
            User = UserDto.FromDatabaseObject(model.User!),
            Club = ClubDto.FromDatabaseObject(model.Club!),
            IsAdmin = model.IsAdmin
        };
    }
}