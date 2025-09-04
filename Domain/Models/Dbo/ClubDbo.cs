using System.ComponentModel.DataAnnotations;
using Domain.Interfaces.Dbo;

namespace Domain.Models.Dbo;

public class ClubDbo
{
    public Guid Id { get; set; }
    
    [MaxLength(128)] public string Name { get; set; } = null!;
    [MaxLength(256)] public string? Motto { get; set; }
    [MaxLength(256)] public string? ImageUrl { get; set; }
    public bool IsPrivate { get; set; } = true;
    
    public DateTime DateCreated { get; set; }
    public Guid? CreatedById { get; set; }

    public ICollection<ClubMembershipDbo> ClubMemberships { get; set; } = [];
    public ICollection<InvitationDbo> Invitations { get; set; } = [];
}