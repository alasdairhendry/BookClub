using Data.Models.Dbo;

namespace IntegrationTests.Models.DTO.Actions;

public class InvitationCreateDto
{
    public Guid ApplicationUserId { get; set; }
    public Guid ClubId { get; set; }
}