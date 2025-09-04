using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;

namespace Application.Interfaces;

public interface IClubService
{
    Task<ResultStateId> CreateClub(ClubCreateDto model);
    Task<ResultState> UpdateClub(ClubUpdateDto model);
    Task<ResultState> DeleteClub(Guid? id);
    Task<ResultState<ClubDto?>> GetClub(Guid? id);
    Task<ResultState<List<ClubDto>>> GetClubs();
    Task<ResultState<List<ClubMembershipDto>>> GetMemberships(Guid? id);
    Task<ResultState> RemoveMemberFromClub(Guid? userId, Guid? clubId);
    Task<ResultState> UpdateMemberRole(Guid? userId, Guid? clubId, bool isAdmin);
}