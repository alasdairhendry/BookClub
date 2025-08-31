using Data;
using Domain.DataAccess;
using Domain.Interfaces;
using Domain.Models.DTO;
using Domain.Models.State;

namespace Domain.Services;

public class AccountService : IAccountService
{
    private readonly ApplicationDbContext _dbContext;

    public AccountService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ResultState<UserDto?>> GetUserDetails(Guid? id)
    {
        try
        {
            using var work = new UnitOfWork(_dbContext);

            var user = await work.ApplicationUserRepository.GetByIDAsync(id);

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

    public async Task<ResultState<List<ClubDto>>> GetUserClubMemberships(Guid? id)
    {
        try
        {
            using var work = new UnitOfWork(_dbContext);

            var user = await work.ApplicationUserRepository.FilterAsSingleAsync(x=>x.Id == id, "ClubMemberships");

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
}