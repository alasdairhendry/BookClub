using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class ClubDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string? Motto { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPrivate { get; set; } = true;

    public List<Guid> MembershipIds { get; set; } = [];

    public static ClubDto FromDatabaseObject(ClubDbo model)
    {
        return new ClubDto
        {
            Id = model.Id,
            Name = model.Name,
            Motto = model.Motto,
            IsPrivate = model.IsPrivate,
            ImageUrl = model.ImageUrl,
            MembershipIds = model.ClubMemberships.Select(x => x.Id).ToList(),
        };
    }
}