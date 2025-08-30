using Data.Models.Dbo;
using Domain.Interfaces;
using Domain.Models.State;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Domain.Services;

public class HttpContextService : IHttpContextService
{
    private readonly SignInManager<ApplicationUserDbo> _signInManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public HttpContextService(SignInManager<ApplicationUserDbo> signInManager, IHttpContextAccessor contextAccessor)
    {
        _signInManager = signInManager;
        _contextAccessor = contextAccessor;
    }
    
    /// <summary>
    /// Is the context user active, authenticated, not blocked etc.
    /// Essentially, free to make actions on the app (create clubs, comments, etc.)
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<ResultState<ApplicationUserDbo?>> ContextUserIsActiveAsync()
    {
        try
        {
            var claimsPrincipal = _contextAccessor.HttpContext?.User;

            if (claimsPrincipal?.Identity is null)
                return ResultState<ApplicationUserDbo?>.Failed(null, "User not authenticated");

            if (claimsPrincipal.Identity?.IsAuthenticated == false)
                return ResultState<ApplicationUserDbo?>.Failed(null, "User not authenticated");

            var identityUser = await _signInManager.UserManager.GetUserAsync(claimsPrincipal);

            if (identityUser == null)
                return ResultState<ApplicationUserDbo?>.Failed(null, "User not authenticated");

            // TODO - Uncomment once testing concluded
            // if (identityUser.EmailConfirmed == false)
            //     return ResultState<ApplicationUserDbo?>.Failed(null, "Email has not been confirmed yet");
            //
            // if (identityUser.LockoutEnd >= DateTime.UtcNow)
            //     return ResultState<ApplicationUserDbo?>.Failed(null, "Account is temporarily locked");
            
            return ResultState<ApplicationUserDbo?>.Success(identityUser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultState<ApplicationUserDbo?>.Failed(null, "An error has occurred");
        }
    }
}