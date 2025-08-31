using Domain.Models.DTO;
using Domain.Models.DTO.Actions;
using Domain.Models.DTO.Objects;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IClubService
{
    Task<ResultState<Guid?>> CreateClub(ClubCreateDto model);
    Task<ResultState> UpdateClub(ClubUpdateDto model);
    Task<ResultState> DeleteClub(Guid? id);
    Task<ResultState<ClubDto?>> GetClub(Guid? id);
    Task<ResultState<List<ClubDto>>> GetClubs();
    Task<ResultState<List<ClubMembershipDto>>> GetMemberships(Guid? id);
    Task<ResultState> RemoveMemberFromClub(Guid? userId, Guid? clubId);
    Task<ResultState> UpdateMemberRole(Guid? userId, Guid? clubId, bool isAdmin);
}