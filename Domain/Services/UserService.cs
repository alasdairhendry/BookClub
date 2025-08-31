using System.Text.RegularExpressions;
using Data;
using Data.Models.Dbo;
using Domain.DataAccess;
using Domain.Interfaces;
using Domain.Models.DTO;
using Domain.Models.DTO.Actions;
using Domain.Models.State;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Domain.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SignInManager<ApplicationUserDbo> _signInManager;

    public UserService(ApplicationDbContext dbContext, SignInManager<ApplicationUserDbo> signInManager)
    {
        _dbContext = dbContext;
        _signInManager = signInManager;
    }

    public async Task<ResultState> Register(UserRegistrationModel model)
    {
        try
        {
            if (Regex.IsMatch(model.Email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == false)
                return ResultState.Failed("Email is invalid");

            if (model.Password != model.ConfirmPassword)
                return ResultState.Failed("Passwords do not match");

            var user = new ApplicationUserDbo { UserName = model.Username, Email = model.Email, DateCreated = DateTime.UtcNow, LastLogin = DateTime.UtcNow };
            var result = await _signInManager.UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return ResultState.Success();

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateEmail)) ||
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

    public async Task<ResultState<Guid?>> Login(UserLoginModel model)
    {
        try
        {
            _signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);

            if (user is null || user.UserName is null)
                return ResultState<Guid?>.Failed(null, "User not found");
            
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);

            using var work = new UnitOfWork(_dbContext);
            var userStore = await work.ApplicationUserRepository.FilterAsSingleAsync(x => x.Email == model.Email);

            if (userStore == null)
                return ResultState<Guid?>.Failed(null, "User not found");

            if (result.Succeeded)
                return ResultState<Guid?>.Success(userStore.Id);

            if (result.IsLockedOut)
                return ResultState<Guid?>.Failed(null, "Account is temporarily locked");

            if (result.IsNotAllowed)
                return ResultState<Guid?>.Failed(null, "Email has not been confirmed yet");

            if (result.Succeeded == false)
                return ResultState<Guid?>.Failed(null, "User not found");

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

            if (result.Succeeded)
                return ResultState<UserTokenModel>.Success(null!);

            if (result.IsLockedOut)
                return ResultState<UserTokenModel>.Failed(null!, "Account is temporarily locked");

            if (result.IsNotAllowed)
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