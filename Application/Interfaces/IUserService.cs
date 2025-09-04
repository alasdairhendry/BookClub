using Application.Models.Dto.Actions;
using Application.Models.State;

namespace Application.Interfaces;

public interface IUserService
{
    Task<ResultStateId> Register(UserRegistrationDto model);
    Task<ResultStateId> Login(UserLoginDto model);
    Task<ResultState<UserTokenDto>> GetToken(UserLoginDto model);
    Task<ResultState> RegisterWithSpecificId(UserRegistrationDto model, Guid userId);
}