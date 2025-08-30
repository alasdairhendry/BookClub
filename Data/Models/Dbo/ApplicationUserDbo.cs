using Microsoft.AspNetCore.Identity;

namespace Data.Models.Dbo;

public class ApplicationUserDbo : IdentityUser<Guid>
{
    public DateTime DateCreated { get; set; }
    public DateTime LastLogin { get; set; }

    public ICollection<Guid> ClubMembershipIds { get; set; } = [];
    
    public ICollection<CommentDbo> Comments { get; set; } = [];
    public ICollection<InvitationDbo> Invitations { get; set; } = [];
}