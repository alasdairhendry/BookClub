using Domain.Models.DTO;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IAccountService
{
    Task<ResultState<UserDto?>> GetUserDetails(Guid? id);
    Task<ResultState<List<ClubDto>>> GetUserClubMemberships(Guid? id);
}