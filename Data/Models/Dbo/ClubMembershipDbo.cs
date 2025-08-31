namespace Data.Models.Dbo;

public class ClubMembershipDbo
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public ApplicationUserDbo? User { get; set; }
    
    public Guid ClubId { get; set; }
    public ClubDbo? Club { get; set; }
    
    public bool IsAdmin { get; set; } = false;
}