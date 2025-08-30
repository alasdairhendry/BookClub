namespace Data.Models.Dbo;

public class InvitationDbo
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    public Guid ClubId { get; set; }
}