using Domain.Models.DTO;
using Domain.Models.DTO.Objects;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IAccountService
{
    Task<ResultState<UserDto?>> GetUserDetailsAsync(Guid? userId);
    Task<ResultState<List<ClubDto>>> GetUserClubMembershipsAsync(Guid? userId);
    Task<ResultState> IsMemberOfClubAsync(Guid? userId, Guid clubId);
    Task<ResultState<List<InvitationDto>>> GetUserClubInvitationsAsync(Guid? userId);
}