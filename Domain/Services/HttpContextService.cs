using Data.Models.Dbo;
using Domain.Enums;
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

    public async Task<ResultState<ApplicationUserDbo?>> GetContextApplicationUserAsync()
    {
        try
        {
            var claimsPrincipal = _contextAccessor.HttpContext?.User;

            if(claimsPrincipal == null)
                return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Exception, "Context not found");
            
            // TODO - Unsure if we want to verify authentication at this point?
            // if (claimsPrincipal?.Identity is null)
                // return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Unauthorised, "User not authenticated");

            // if (claimsPrincipal.Identity?.IsAuthenticated == false)
                // return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Unauthorised, "User not authenticated");

            var applicationUser = await _signInManager.UserManager.GetUserAsync(claimsPrincipal);

            if (applicationUser == null)
                return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.NotFound, "User does not exist");
            
            return ResultState<ApplicationUserDbo?>.Success(applicationUser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Exception, "An error has occurred");
        }
    }

    /// <summary>
    /// Is the context user active, authenticated, not blocked etc.
    /// Essentially, free to make actions on the app (create clubs, comments, etc.)
    /// </summary>
    public async Task<ResultState<ApplicationUserDbo?>> ContextApplicationUserIsEnabledAsync()
    {
        try
        {
            var claimsPrincipal = _contextAccessor.HttpContext?.User;

            if (claimsPrincipal?.Identity is null)
                return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Unauthorised, "User not authenticated");

            if (claimsPrincipal.Identity?.IsAuthenticated == false)
                return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Unauthorised, "User not authenticated");

            var applicationUser = await _signInManager.UserManager.GetUserAsync(claimsPrincipal);

            if (applicationUser == null)
                return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Unauthorised, "User not authenticated");

            // TODO - Uncomment once testing concluded
            // if (identityUser.EmailConfirmed == false)
            //     return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Validation, "Email has not been confirmed yet");
            //
            // if (identityUser.LockoutEnd >= DateTime.UtcNow)
            //     return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Validation, "Account is temporarily locked");
            
            return ResultState<ApplicationUserDbo?>.Success(applicationUser);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultState<ApplicationUserDbo?>.Failed(null, ResultErrorType.Exception, "An error has occurred");
        }
    }
}