using Domain.Models.DTO;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IClubService
{
    Task<ResultState<Guid?>> CreateClub(ClubCreateDto model);
    Task<ResultState> UpdateClub(ClubUpdateDto model);
    Task<ResultState> DeleteClub(Guid? id);
    Task<ResultState<ClubDto?>> GetClub(Guid? id);
    Task<ResultState<List<ClubDto>>> GetClubs();
}