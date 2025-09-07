using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;

namespace Application.Interfaces;

public interface IClubService
{
    Task<ResultStateId> CreateClub(ClubCreateDto model);
    Task<ResultState> UpdateClub(ClubUpdateDto model, Guid clubId);
    Task<ResultState> DeleteClub(Guid? id);
    Task<ResultState<ClubDto?>> GetClub(Guid? id);
    Task<ResultState<List<ClubDto>>> GetClubs();
    Task<ResultState<List<ClubMembershipDto>>> GetMemberships(Guid? id);
    Task<ResultState> RemoveMemberFromClub(Guid? userId, Guid? clubId);
    Task<ResultState> UpdateMemberRole(Guid? userId, Guid? clubId, bool isAdmin);
    Task<ResultState<List<ActivityDto>>> GetActivities(Guid? id, bool includeActive);
    Task<ResultState<List<InvitationDto>>> GetInvitations(Guid? id, bool includeInactive);
}