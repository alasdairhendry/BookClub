using Data.Models.Dbo;
using Domain.Models.State;

namespace Domain.Interfaces;

public interface IHttpContextService
{
    /// <summary>
    /// Obtains the IdentityUser from the HttpContext
    /// </summary>
    Task<ResultState<ApplicationUserDbo?>> GetContextApplicationUserAsync();
    
    /// <summary>
    /// Is the context user active, authenticated, not blocked etc.
    /// Essentially, free to make actions on the app (create clubs, comments, etc.)
    /// </summary>
    Task<ResultState<ApplicationUserDbo?>> ContextApplicationUserIsEnabledAsync();
}