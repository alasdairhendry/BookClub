using Domain.Enums;
using Application.Interfaces;
using Application.Models.Dto.Objects;
using Application.Models.State;
using Data.Repositories;
using Domain.Models.Dbo;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextService _httpContextService;

    public AccountService(IUnitOfWork unitOfWork, IServiceProvider serviceProvider, IHttpContextService httpContextService)
    {
        _unitOfWork = unitOfWork;
        _serviceProvider = serviceProvider;
        _httpContextService = httpContextService;
    }

    public async Task<ResultState<UserDto?>> GetUserDetailsAsync(Guid? userId)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<ApplicationUserDbo>().GetAsync(userId);

            if (user is null)
                return ResultState<UserDto?>.Failed(ResultErrorType.NotFound, "User does not exist");

            var dto = new UserDto()
            {
                Id = user.Id,
                EmailAddress = user.Email!,
                Username = user.UserName!,
                DateCreated = user.DateCreated,
                LastLogin = user.LastLogin,
            };

            return ResultState<UserDto?>.Success(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// TODO - Update this to return a list of the actual memberships
    /// So
    /// [
    ///     {
    ///         club: {...}
    ///         membership: {...}
    ///     },
    ///     {
    ///         club: {...}
    ///         membership: {...}
    ///     }
    /// ] 
    public async Task<ResultState<List<ClubDto>>> GetUserClubMembershipsAsync(Guid? userId)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<ApplicationUserDbo>().QueryAsSingleAsync(x => x.Id == userId, "ClubMemberships");

            if (user is null)
                return ResultState<List<ClubDto>>.Failed([], ResultErrorType.NotFound, "User does not exist");

            var memberships = new List<ClubDto>();

            // Todo - batch this
            foreach (var membership in user.ClubMemberships)
            {
                if (membership is null)
                    continue;

                var club = await _unitOfWork.GetRepository<ClubDbo>().GetAsync(membership.ClubId);

                if (club is null)
                    continue;

                memberships.Add(ClubDto.FromDatabaseObject(club));
            }

            return ResultState<List<ClubDto>>.Success(memberships);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<List<InvitationDto>>> GetUserClubInvitationsAsync(Guid? userId, bool activeOnly = false)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<ApplicationUserDbo>().QueryAsSingleAsync(x => x.Id == userId, "Invitations.TargetClub,Invitations.FromUser");

            if (user is null)
                return ResultState<List<InvitationDto>>.Failed([], ResultErrorType.NotFound, "User does not exist");

            var response = new List<InvitationDto>();

            var invitations = user.Invitations;

            if (activeOnly)
                invitations = user.Invitations.Where(x => x.DateResponded == null).ToList();

            // Todo - batch this
            foreach (var invitation in invitations)
            {
                if (invitation is null)
                    continue;

                response.Add(InvitationDto.FromDatabaseObject(invitation));
            }

            return ResultState<List<InvitationDto>>.Success(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> LeaveClub(Guid? clubId)
    {
        try
        {
            var userResult = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (userResult.Succeeded == false || userResult.Data is null)
                return ResultState.Failed(userResult.ErrorType, userResult.PublicMessage);

            var membership = await _unitOfWork.GetRepository<ClubMembershipDbo>().QueryAsSingleAsync(x => x.ClubId == clubId && x.UserId == userResult.Data.Id);

            if (membership is null)
                return ResultState.Failed(ResultErrorType.Conflict, "User is not a member of this Club");

            if (membership.IsAdmin)
            {
                var adminCount = await _unitOfWork.GetRepository<ClubMembershipDbo>().GetCount(x => x.ClubId == clubId && x.IsAdmin);

                if (adminCount == 1)
                    return ResultState.Failed(ResultErrorType.Validation, "Cannot leave Club when you are the only admin");
            }

            var memberCount = await _unitOfWork.GetRepository<ClubMembershipDbo>().GetCount(x => x.ClubId == clubId);

            if (memberCount == 1)
                return ResultState.Failed(ResultErrorType.Validation, "Cannot leave Club when you are the only member");
            
            _unitOfWork.GetRepository<ClubMembershipDbo>().Delete(membership);
            await _unitOfWork.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> IsMemberOfClubAsync(Guid? userId, Guid clubId)
    {
        try
        {
            // await using var scope = _serviceProvider.CreateAsyncScope();
            // var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var scope = _serviceProvider.CreateScope();
            var _scopedUnitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            if (await _scopedUnitOfWork.GetRepository<ClubMembershipDbo>().QueryAsSingleAsync(x => x.ClubId == clubId && x.UserId == userId) is not null)
                return ResultState.Success();

            return ResultState.Failed();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}