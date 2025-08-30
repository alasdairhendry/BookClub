using Domain.Models.DTO;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IUserService
{
    Task<ResultState> Register(UserRegistrationModel model);
    Task<ResultState> Login(UserLoginModel model);
    Task<ResultState<UserTokenModel>> GetToken(UserLoginModel model);

    /// <summary>
    /// Is the context user active, authenticated, not blocked etc.
    /// Essentially, free to make actions on the app (create clubs, comments, etc.)
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<ResultState> ContextUserIsActiveAsync(Guid userId);
}