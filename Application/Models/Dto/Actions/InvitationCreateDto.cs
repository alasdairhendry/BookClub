namespace Application.Models.Dto.Actions;

public class InvitationCreateDto
{
    public Guid ApplicationUserId { get; set; }
    public Guid ClubId { get; set; }
}