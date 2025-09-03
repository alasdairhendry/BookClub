using Data.Models.Dbo;
using Domain.Models.DTO;
using Domain.Models.DTO.Actions;
using Domain.Models.DTO.Objects;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IUserService
{
    Task<ResultStateId> Register(UserRegistrationDto model);
    Task<ResultStateId> Login(UserLoginDto model);
    Task<ResultState<UserTokenDto>> GetToken(UserLoginDto model);
    Task<ResultState> RegisterWithSpecificId(UserRegistrationDto model, Guid userId);
}