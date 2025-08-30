namespace Domain.Models.DTO;

public class ClubDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = null!;
    public string? Motto { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPrivate { get; set; } = true;

    public List<Guid> MembershipIds { get; set; } = [];
}