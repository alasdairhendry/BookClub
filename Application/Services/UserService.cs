using System.Text.RegularExpressions;
using Domain.Models.Dbo;
using Data.Repositories;
using Domain.Enums;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Application.Models.State;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly SignInManager<ApplicationUserDbo> _signInManager;

    public UserService(IUnitOfWork unitOfWork, SignInManager<ApplicationUserDbo> signInManager)
    {
        _unitOfWork = unitOfWork;
        _signInManager = signInManager;
    }

    public async Task<ResultStateId> Register(UserRegistrationDto model)
    {
        try
        {
            if (Regex.IsMatch(model.Email, "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*") == false)
                return ResultStateId.Failed(ResultErrorType.Validation, "Email is invalid");

            if (model.Password != model.ConfirmPassword)
                return ResultStateId.Failed(ResultErrorType.Validation, "Passwords do not match");

            var user = new ApplicationUserDbo { UserName = model.Username, Email = model.Email, DateCreated = DateTime.UtcNow, LastLogin = DateTime.UtcNow };
            var result = await _signInManager.UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return ResultStateId.Success(user.Id);

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
                return ResultStateId.Failed(ResultErrorType.Validation, "Username already exists");

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.InvalidUserName)))
                return ResultStateId.Failed(ResultErrorType.Validation, "Username is invalid");

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.DuplicateEmail)))
                return ResultStateId.Failed(ResultErrorType.Validation, "Email address already exists");

            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.InvalidEmail)))
                return ResultStateId.Failed(ResultErrorType.Validation, "Email address is not valid");

            // TODO - Define password rules
            if (result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresDigit)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresLower)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresUpper)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordTooShort)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric)) ||
                result.Errors.Any(x => x.Code == nameof(IdentityErrorDescriber.PasswordRequiresUniqueChars)))
                return ResultStateId.Failed(ResultErrorType.Validation, "Password must contain [xxx]");

            throw new Exception(JsonConvert.SerializeObject(result.Errors.ToList()));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> RegisterWithSpecificId(UserRegistrationDto model, Guid userId)
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

    public async Task<ResultStateId> Login(UserLoginDto model)
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

            var userStore = await _unitOfWork.GetRepository<ApplicationUserDbo>().QueryAsSingleAsync(x => x.Email == model.Email);

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

    public async Task<ResultState<UserTokenDto>> GetToken(UserLoginDto model)
    {
        try
        {
            _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);

            if (result.Succeeded)
                return ResultState<UserTokenDto>.Success(null!);

            if (result.IsLockedOut)
                return ResultState<UserTokenDto>.Failed(null!, ResultErrorType.Validation, "Account is temporarily locked");

            if (result.IsNotAllowed)
                return ResultState<UserTokenDto>.Failed(null!, ResultErrorType.Validation, "Email has not been confirmed yet");

            throw new Exception(JsonConvert.SerializeObject(result));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}