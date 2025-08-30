using System.Text.RegularExpressions;
using Domain.Interfaces;
using Domain.Models.DTO;
using Domain.Models.State;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Domain.Services;

public class UserService : IUserService
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(SignInManager<IdentityUser> signInManager, IHttpContextAccessor contextAccessor)
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
    public async Task<ResultState> ContextUserIsActiveAsync(Guid userId)
    {
        try
        {
            var claimsPrincipal = _contextAccessor.HttpContext?.User;

            if (claimsPrincipal?.Identity is null)
                return ResultState.Failed("User not authenticated");

            if (claimsPrincipal.Identity?.IsAuthenticated == false)
                return ResultState.Failed("User not authenticated");

            var identityUser = await _signInManager.UserManager.GetUserAsync(claimsPrincipal);

            if (identityUser == null)
                return ResultState.Failed("User not authenticated");

            if (identityUser.EmailConfirmed == false)
                return ResultState.Failed("Email has not been confirmed yet");
            
            if (identityUser.LockoutEnd >= DateTime.UtcNow)
                return ResultState.Failed("Account is temporarily locked");
            
            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return ResultState.Failed("An error has occurred");
        }
    }

    public async Task<ResultState> Register(UserRegistrationModel model)
    {
        try
        {
            if (Regex.IsMatch(model.Email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == false)
                return ResultState.Failed("Email is invalid");
                
            if(model.Password != model.ConfirmPassword)
                return ResultState.Failed("Passwords do not match");
            
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _signInManager.UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return ResultState.Success();
            
            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateEmail))||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
                return ResultState.Failed("Email address already exists");

            // TODO - Define password rules
            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresDigit)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresLower)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresUpper)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordTooShort)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars)))
                return ResultState.Failed("Password must contain [xxx]");

            throw new Exception(JsonConvert.SerializeObject(result.Errors.ToList()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> Login(UserLoginModel model)
    {
        try
        {
            _signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
            
            if(result.Succeeded)
                return ResultState.Success();
            
            if(result.IsLockedOut)
                return ResultState.Failed("Account is temporarily locked");
            
            if(result.IsNotAllowed)
                return ResultState.Failed("Email has not been confirmed yet");
            
            throw new Exception(JsonConvert.SerializeObject(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<UserTokenModel>> GetToken(UserLoginModel model)
    {
        try
        {
            _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
            
            if(result.Succeeded)
                return ResultState<UserTokenModel>.Success(null!);
            
            if(result.IsLockedOut)
                return ResultState<UserTokenModel>.Failed(null!, "Account is temporarily locked");
            
            if(result.IsNotAllowed)
                return ResultState<UserTokenModel>.Failed(null!, "Email has not been confirmed yet");
            
            throw new Exception(JsonConvert.SerializeObject(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}