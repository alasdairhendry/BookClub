using Data;
using Domain.DataAccess;
using Domain.Interfaces;
using Domain.Models.DTO.Objects;
using Domain.Models.State;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextService _httpContextService;

    public AccountService(ApplicationDbContext dbContext, IServiceProvider serviceProvider, IHttpContextService httpContextService)
    {
        _dbContext = dbContext;
        _serviceProvider = serviceProvider;
        _httpContextService = httpContextService;
    }

    public async Task<ResultState<UserDto?>> GetUserDetailsAsync(Guid? userId)
    {
        try
        {
            using var work = new UnitOfWork(_dbContext);

            var user = await work.ApplicationUserRepository.GetByIDAsync(userId);

            if (user is null)
                return ResultState<UserDto?>.Failed(null, "User does not exist");

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
            using var work = new UnitOfWork(_dbContext);

            var user = await work.ApplicationUserRepository.FilterAsSingleAsync(x=>x.Id == userId, "ClubMemberships");

            if (user is null)
                return ResultState<List<ClubDto>>.Failed([], "User does not exist");

            var memberships = new List<ClubDto>();
            
            // Todo - batch this
            foreach (var membership in user.ClubMemberships)
            {
                if (membership is null)
                    continue;

                var club = await work.ClubRepository.GetByIDAsync(membership.ClubId);
                
                if(club is null)
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
            using var work = new UnitOfWork(_dbContext);

            var user = await work.ApplicationUserRepository.FilterAsSingleAsync(x=>x.Id == userId, "Invitations.TargetClub,Invitations.FromUser");

            if (user is null)
                return ResultState<List<InvitationDto>>.Failed([], "User does not exist");

            var response = new List<InvitationDto>();

            var invitations = user.Invitations;
            
            if(activeOnly)
                invitations = user.Invitations.Where(x=>x.DateResponded == null).ToList();
            
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
            var userResult = await _httpContextService.ContextUserIsActiveAsync();

            if (userResult.Succeeded == false || userResult.Data is null)
                return ResultState.Failed(userResult.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            var membership = await work.ClubMembershipRepository.FilterAsSingleAsync(x=>x.ClubId == clubId && x.UserId == userResult.Data.Id);

            if (membership is null)
                return ResultState.Failed("User is not a member of this club");

            work.ClubMembershipRepository.Delete(membership);
            await work.SaveAsync();

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
            await using var scope = _serviceProvider.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            using var work = new UnitOfWork(dbContext);
            
            if (await work.ClubMembershipRepository.FilterAsSingleAsync(x => x.ClubId == clubId && x.UserId == userId) is not null)
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