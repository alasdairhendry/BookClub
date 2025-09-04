using Data;
using Data.Repositories;
using Domain.Enums;
using Application.Interfaces;
using Application.Models.State;
using Domain.Models.Dbo;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public class PermissionService : IPermissionService
{
    private readonly IServiceProvider _provider;
    private readonly IHttpContextService _httpContextService;

    public PermissionService(IServiceProvider provider, IHttpContextService httpContextService)
    {
        _provider = provider;
        _httpContextService = httpContextService;
    }

    public async Task<ResultState> ContextUserHasViewOfClubAsync(Guid? clubId)
    {
        try
        {
            var user = await _httpContextService.GetContextApplicationUserAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            return await UserHasViewOfClubAsync(user.Data.Id, clubId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> ContextUserIsMemberOfClubAsync(Guid? clubId)
    {
        try
        {
            var user = await _httpContextService.GetContextApplicationUserAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            return await UserIsMemberOfClubAsync(user.Data.Id, clubId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> ContextUserIsAdminOfClubAsync(Guid? clubId)
    {
        try
        {
            var user = await _httpContextService.GetContextApplicationUserAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            return await UserIsAdminOfClubAsync(user.Data.Id, clubId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> UserHasViewOfClubAsync(Guid? userId, Guid? clubId)
    {
        try
        {
            var scope = _provider.CreateScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
            // using var work = new UnitOfWork(dbContext);

            var user = await _unitOfWork.GetRepository<ApplicationUserDbo>().GetAsync(userId);
            var club = await _unitOfWork.GetRepository<ClubDbo>().GetAsync(clubId);
            var memberships = await _unitOfWork.GetRepository<ClubMembershipDbo>().QueryAsync(x => x.ClubId == clubId);

            if (user is null)
                return ResultState.Failed(ResultErrorType.NotFound, "User not found");

            if (club is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Club not found");

            // If User is a Member, they can view the Club
            if (memberships.FirstOrDefault(x => x.UserId == user.Id) != null)
                return ResultState.Success();

            if (club.IsPrivate)
                return ResultState.Failed(ResultErrorType.Unauthorised, "User does not have access to this Club");
            else
                return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> UserIsMemberOfClubAsync(Guid? userId, Guid? clubId)
    {
        try
        {
            var scope = _provider.CreateScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
            // using var work = new UnitOfWork(dbContext);

            var user = await _unitOfWork.GetRepository<ApplicationUserDbo>().GetAsync(userId);
            var club = await _unitOfWork.GetRepository<ClubDbo>().GetAsync(clubId);
            var memberships = await _unitOfWork.GetRepository<ClubMembershipDbo>().QueryAsync(x => x.ClubId == clubId);

            if (user is null)
                return ResultState.Failed(ResultErrorType.NotFound, "User not found");

            if (club is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Club not found");

            if (memberships.FirstOrDefault(x => x.UserId == user.Id) == null)
                return ResultState.Failed(ResultErrorType.Unauthorised, "User is not a member of this Club");

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> UserIsAdminOfClubAsync(Guid? userId, Guid? clubId)
    {
        try
        {
            var scope = _provider.CreateScope();
            var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
            // using var work = new UnitOfWork(dbContext);

            var user = await _unitOfWork.GetRepository<ApplicationUserDbo>().GetAsync(userId);
            var club = await _unitOfWork.GetRepository<ClubDbo>().GetAsync(clubId);
            var memberships = await _unitOfWork.GetRepository<ClubMembershipDbo>().QueryAsync(x => x.ClubId == clubId);

            if (user is null)
                return ResultState.Failed(ResultErrorType.NotFound, "User not found");

            if (club is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Club not found");

            if (memberships.FirstOrDefault(x => x.UserId == user.Id && x.IsAdmin) == null)
                return ResultState.Failed(ResultErrorType.Unauthorised, "User does not have admin access to this Club");

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}