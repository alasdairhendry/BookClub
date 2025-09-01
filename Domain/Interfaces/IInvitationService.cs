using Domain.Models.DTO.Actions;
using Domain.Models.DTO.Objects;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IInvitationService
{
    Task<ResultStateId> SendInvitation(InvitationCreateDto model);
    Task<ResultState> AcceptInvitation(Guid? invitationId);
    Task<ResultState> DeclineInvitation(Guid? invitationId);
}