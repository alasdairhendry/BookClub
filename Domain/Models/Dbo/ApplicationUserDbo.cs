using System.ComponentModel.DataAnnotations;
using Domain.Interfaces.Dbo;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Dbo;

public class ApplicationUserDbo : IdentityUser<Guid>
{
    public DateTime DateCreated { get; set; }
    public DateTime LastLogin { get; set; }

    public ICollection<ClubMembershipDbo> ClubMemberships { get; set; } = [];

    public ICollection<DiscussionDbo> Discussions { get; set; } = [];
    public ICollection<CommentDbo> Comments { get; set; } = [];
    
    public ICollection<InvitationDbo> Invitations { get; set; } = [];
}