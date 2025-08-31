using Data.Models.Dbo;
using Domain.Models.DTO;
using Domain.Models.DTO.Actions;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IUserService
{
    Task<ResultState> Register(UserRegistrationModel model);
    Task<ResultState<Guid?>> Login(UserLoginModel model);
    Task<ResultState<UserTokenModel>> GetToken(UserLoginModel model);
}