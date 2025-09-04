using Application.Models.Dto.Objects;
using Application.Models.State;

namespace Application.Interfaces;

public interface IAccountService
{
    Task<ResultState<UserDto?>> GetUserDetailsAsync(Guid? userId);
    Task<ResultState<List<ClubDto>>> GetUserClubMembershipsAsync(Guid? userId);
    Task<ResultState<List<InvitationDto>>> GetUserClubInvitationsAsync(Guid? userId, bool activeOnly = false);
    Task<ResultState> IsMemberOfClubAsync(Guid? userId, Guid clubId);
    Task<ResultState> LeaveClub(Guid? clubId);
}