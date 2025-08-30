using Data;
using Data.Models.Dbo;
using Domain.DataAccess;
using Domain.Interfaces;
using Domain.Models.DTO;
using Domain.Models.State;

namespace Domain.Services;

public class ClubService : IClubService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextService _httpContextService;
    private readonly IPermissionService _permissionService;

    public ClubService(ApplicationDbContext dbContext, IHttpContextService httpContextService, IPermissionService permissionService)
    {
        _httpContextService = httpContextService;
        _permissionService = permissionService;
        _dbContext = dbContext;
    }

    public async Task<ResultState<ClubDto?>> GetClub(Guid? id)
    {
        try
        {
            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<ClubDto?>.Failed(null, user.PublicMessage);

            if ((await _permissionService.ContextUserHasAccessToAsync(id)).Succeeded == false)
                return ResultState<ClubDto?>.Failed(null, "User does not have access to this resource");

            using var work = new UnitOfWork(_dbContext);
            var result = await work.ClubRepository.GetByIDAsync(id);

            if (result is null)
                return ResultState<ClubDto?>.Failed(null, "Club not found");

            ClubDto club = new ClubDto
            {
                Id = result.Id,
                Name = result.Name,
                Motto = result.Motto,
                IsPrivate = result.IsPrivate,
                ImageUrl = result.ImageUrl,
                MembershipIds = result.ClubMemberships.Select(x => x.Id).ToList(),
            };

            return ResultState<ClubDto?>.Success(club);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<List<ClubDto>>> GetClubs()
    {
        try
        {
            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<List<ClubDto>>.Failed([], user.PublicMessage);

            using var work = new UnitOfWork(_dbContext);
            var result = await work.ClubRepository.GetAsync(includeProperties: "ClubMemberships");

            List<ClubDto> clubs = result.Take(20).Select(x => new ClubDto()
            {
                Id = x.Id,
                Name = x.Name,
                Motto = x.Motto,
                IsPrivate = x.IsPrivate,
                ImageUrl = x.ImageUrl,
                MembershipIds = x.ClubMemberships.Select(y => y.Id).ToList(),
            }).ToList();

            return ResultState<List<ClubDto>>.Success(clubs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<Guid?>> CreateClub(ClubCreateDto model)
    {
        try
        {
            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<Guid?>.Failed(null, user.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            ClubDbo club = new ClubDbo
            {
                Name = model.Name,
                Motto = model.Motto,
                ImageUrl = model.ImageUrl,
                IsPrivate = model.IsPrivate,
                DateCreated = DateTime.UtcNow,
                CreatedById = user.Data.Id,
                ClubMemberships = []
            };

            await work.ClubRepository.InsertAsync(club);

            ClubMembershipDbo membership = new ClubMembershipDbo
            {
                ClubId = club.Id,
                UserId = user.Data.Id,
                IsAdmin = true
            };

            await work.ClubMembershipRepository.InsertAsync(membership);
            await work.SaveAsync();

            return ResultState<Guid?>.Success(club.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> UpdateClub(ClubUpdateDto model)
    {
        try
        {
            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.PublicMessage);

            if ((await _permissionService.ContextUserHasAccessToAsync(model.Id)).Succeeded == false)
                return ResultState.Failed("User does not have access to this resource");

            using var work = new UnitOfWork(_dbContext);

            ClubDbo? club = await work.ClubRepository.GetByIDAsync(model.Id);

            if (club is null)
                return ResultState.Failed("Club not found");

            if (string.IsNullOrWhiteSpace(model.Name))
                return ResultState.Failed("Club must have a name");

            club.Name = model.Name;
            club.Motto = model.Motto;
            club.ImageUrl = model.ImageUrl;
            club.IsPrivate = model.IsPrivate;

            work.ClubRepository.Update(club);
            await work.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> DeleteClub(Guid? id)
    {
        try
        {
            if (id == null)
                return ResultState.Failed("Club not found");

            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.PublicMessage);

            if ((await _permissionService.ContextUserIsAdminOfAsync(id)).Succeeded == false)
                return ResultState.Failed("User does not have admin access to this resource");

            using var work = new UnitOfWork(_dbContext);
            await work.ClubRepository.DeleteAsync(id);
            await work.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}