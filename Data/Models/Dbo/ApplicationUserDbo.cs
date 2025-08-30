using Microsoft.AspNetCore.Identity;

namespace Data.Models.Dbo;

public class ApplicationUserDbo : IdentityUser<Guid>
{
    public DateTime DateCreated { get; set; }
    public DateTime LastLogin { get; set; }
    
    public List<ClubMembershipDbo> ClubMemberships { get; set; }
    public ICollection<CommentDbo> Comments { get; set; } = [];
    public ICollection<InvitationDbo> Invitations { get; set; } = [];
}