using Data;
using Domain.DataAccess;
using Domain.Interfaces;
using Domain.Models.DTO.Objects;
using Domain.Models.State;

namespace Domain.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _dbContext;

    public AccountService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
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
    
    public async Task<ResultState<List<InvitationDto>>> GetUserClubInvitationsAsync(Guid? userId)
    {
        try
        {
            using var work = new UnitOfWork(_dbContext);

            var user = await work.ApplicationUserRepository.FilterAsSingleAsync(x=>x.Id == userId, "Invitations");

            if (user is null)
                return ResultState<List<InvitationDto>>.Failed([], "User does not exist");

            var invitations = new List<InvitationDto>();
            
            // Todo - batch this
            foreach (var invitation in user.Invitations)
            {
                if (invitation is null)
                    continue;
                
                // For some reason we have to do this ?? It must somehow track the changes on the club
                // If we don't do it, the TargetClubId is filled but the navigation property doesnt link up???
                // Weird that the TargetUser and FromUser aren't an issue???
                var club = await work.ClubRepository.GetByIDAsync(invitation.TargetClubId);

                invitations.Add(InvitationDto.FromDatabaseObject(invitation));
            }

            return ResultState<List<InvitationDto>>.Success(invitations);
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
            using var work = new UnitOfWork(_dbContext);
            
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