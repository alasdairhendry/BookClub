using System.Text.RegularExpressions;
using Data;
using Data.Models.Dbo;
using Domain.DataAccess;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.DTO;
using Domain.Models.DTO.Actions;
using Domain.Models.DTO.Objects;
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
                return ResultState.Failed(ResultErrorType.Validation,"Email is invalid");

            if (model.Password != model.ConfirmPassword)
                return ResultState.Failed(ResultErrorType.Validation,"Passwords do not match");

            var user = new ApplicationUserDbo { UserName = model.Username, Email = model.Email, DateCreated = DateTime.UtcNow, LastLogin = DateTime.UtcNow };
            var result = await _signInManager.UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return ResultState.Success();

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
                return ResultState.Failed(ResultErrorType.Validation,"Username already exists");

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.InvalidUserName)))
                return ResultState.Failed(ResultErrorType.Validation,"Username is invalid");

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateEmail)))
                return ResultState.Failed(ResultErrorType.Validation,"Email address already exists");

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.InvalidEmail)))
                return ResultState.Failed(ResultErrorType.Validation,"Email address is not valid");

            // TODO - Define password rules
            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresDigit)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresLower)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresUpper)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordTooShort)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars)))
                return ResultState.Failed(ResultErrorType.Validation,"Password must contain [xxx]");

            throw new Exception(JsonConvert.SerializeObject(result.Errors.ToList()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> RegisterWithSpecificId(UserRegistrationModel model, Guid userId)
    {
        try
        {
            if (Regex.IsMatch(model.Email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == false)
                return ResultState.Failed(ResultErrorType.Validation, "Email is invalid");

            if (model.Password != model.ConfirmPassword)
                return ResultState.Failed(ResultErrorType.Validation, "Passwords do not match");

            var user = new ApplicationUserDbo { Id = userId, UserName = model.Username, Email = model.Email, DateCreated = DateTime.UtcNow, LastLogin = DateTime.UtcNow };
            var result = await _signInManager.UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return ResultState.Success();

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateEmail)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
                return ResultState.Failed(ResultErrorType.Validation, "Email address already exists");

            // TODO - Define password rules
            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresDigit)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresLower)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresUpper)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordTooShort)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars)))
                return ResultState.Failed(ResultErrorType.Validation, "Password must contain [xxx]");

            throw new Exception(JsonConvert.SerializeObject(result.Errors.ToList()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultStateId> Login(UserLoginModel model)
    {
        try
        {
            _signInManager.AuthenticationScheme = IdentityConstants.ApplicationScheme;

            var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);

            if (user is null || user.UserName is null)
                return ResultStateId.Failed(ResultErrorType.NotFound, "User not found");

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, true, false);

            if (result.IsLockedOut)
                return ResultStateId.Failed(ResultErrorType.Validation, "Account is temporarily locked");

            if (result.IsNotAllowed)
                return ResultStateId.Failed(ResultErrorType.Validation, "Email has not been confirmed yet");

            if (result.Succeeded == false)
                return ResultStateId.Failed(ResultErrorType.NotFound, "User not found");

            using var work = new UnitOfWork(_dbContext);
            var userStore = await work.ApplicationUserRepository.FilterAsSingleAsync(x => x.Email == model.Email);

            if (userStore == null)
                return ResultStateId.Failed(ResultErrorType.NotFound, "User not found");

            if (result.Succeeded)
                return ResultStateId.Success(userStore.Id);

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
                return ResultState<UserTokenModel>.Failed(null!, ResultErrorType.Validation, "Account is temporarily locked");

            if (result.IsNotAllowed)
                return ResultState<UserTokenModel>.Failed(null!, ResultErrorType.Validation, "Email has not been confirmed yet");

            throw new Exception(JsonConvert.SerializeObject(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}