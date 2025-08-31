using Domain.Models.DTO.Actions;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IInvitationService
{
    Task<ResultState<Guid?>> SendInvitation(InvitationCreateDto model);
    Task<ResultState> AcceptInvitation(Guid? invitationId);
    Task<ResultState> DeclineInvitation(Guid? invitationId);
}