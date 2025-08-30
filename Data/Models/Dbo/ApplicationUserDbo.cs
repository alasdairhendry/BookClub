using Microsoft.AspNetCore.Identity;

namespace Data.Models.Dbo;

public class ApplicationUserDbo : IdentityUser<Guid>
{
    public List<ClubDbo> Clubs { get; set; }
    public ICollection<CommentDbo> Comments { get; set; }
}