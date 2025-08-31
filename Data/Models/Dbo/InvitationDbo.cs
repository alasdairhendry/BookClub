namespace Data.Models.Dbo;

public class InvitationDbo
{
    public Guid Id { get; set; }
    
    public Guid FromUserId { get; set; }
    public ApplicationUserDbo FromUser { get; set; } = null!;
    
    public Guid TargetUserId { get; set; }
    public ApplicationUserDbo TargetUser { get; set; } = null!;
    
    public Guid TargetClubId { get; set; }
    public ClubDbo TargetClub { get; set; } = null!;
    
    public DateTime DateCreated { get; set; }
    public DateTime? DateResponded { get; set; } = null;

    public bool Response { get; set; }
}