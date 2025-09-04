using Application.Models.Dto.Actions;
using Application.Models.State;

namespace Application.Interfaces;

public interface IInvitationService
{
    Task<ResultStateId> SendInvitation(InvitationCreateDto model);
    Task<ResultState> AcceptInvitation(Guid? invitationId);
    Task<ResultState> DeclineInvitation(Guid? invitationId);
}