using Data;
using Data.Models.Dbo;
using Domain.DataAccess;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.DTO;
using Domain.Models.DTO.Actions;
using Domain.Models.DTO.Objects;
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
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<ClubDto?>.Failed(null, user.ErrorType, user.PublicMessage);

            var viewCheck = await _permissionService.ContextUserHasViewOfClubAsync(id);
            
            if (viewCheck.Succeeded == false)
                return ResultState<ClubDto?>.Failed(viewCheck.ErrorType, viewCheck.PublicMessage);
            
            if ((await _permissionService.ContextUserHasViewOfClubAsync(id)).Succeeded == false)
                return ResultState<ClubDto?>.Failed(null, ResultErrorType.Unauthorised, "User does not have access to this Club");

            using var work = new UnitOfWork(_dbContext);
            var result = await work.ClubRepository.FilterAsSingleAsync(x => x.Id == id, includeProperties: "ClubMemberships");

            if (result is null)
                return ResultState<ClubDto?>.Failed(null, ResultErrorType.NotFound, "Club not found");

            ClubDto club = ClubDto.FromDatabaseObject(result);

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
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<List<ClubDto>>.Failed([], user.ErrorType, user.PublicMessage);

            using var work = new UnitOfWork(_dbContext);
            var result = await work.ClubRepository.GetAsync(includeProperties: "ClubMemberships");

            List<ClubDto> clubs = result.Take(20).Select(ClubDto.FromDatabaseObject).ToList();

            return ResultState<List<ClubDto>>.Success(clubs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<List<ClubMembershipDto>>> GetMemberships(Guid? id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<List<ClubMembershipDto>>.Failed([], user.ErrorType, user.PublicMessage);

            using var work = new UnitOfWork(_dbContext);
            var club = await work.ClubRepository.FilterAsSingleAsync(x => x.Id == id, includeProperties: "ClubMemberships.User");

            if (club is null)
                return ResultState<List<ClubMembershipDto>>.Failed([], ResultErrorType.NotFound, "Club not found");

            var clubMemberships = new List<ClubMembershipDto>();

            foreach (var clubMembership in club.ClubMemberships)
            {
                var membership = await work.ClubMembershipRepository.GetByIDAsync(clubMembership.Id);

                if (membership is null)
                    continue;

                var dto = ClubMembershipDto.FromDatabaseObject(membership);

                clubMemberships.Add(dto);
            }

            return ResultState<List<ClubMembershipDto>>.Success(clubMemberships);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultStateId> CreateClub(ClubCreateDto model)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultStateId.Failed(user.ErrorType, user.PublicMessage);

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
                MemberSince = DateTime.UtcNow,
                IsAdmin = true
            };

            await work.ClubMembershipRepository.InsertAsync(membership);
            await work.SaveAsync();

            return ResultStateId.Success(club.Id);
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
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(model.Id);
            
            if (adminCheck.Succeeded == false)
                return ResultState.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            ClubDbo? club = await work.ClubRepository.GetByIDAsync(model.Id);

            if (club is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Club not found");

            if (string.IsNullOrWhiteSpace(model.Name))
                return ResultState.Failed(ResultErrorType.Validation, "Club must have a name");

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

    public async Task<ResultState> UpdateMemberRole(Guid? userId, Guid? clubId, bool isAdmin)
    {
        try
        {
            var admin = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (admin.Succeeded == false || admin.Data is null)
                return ResultState.Failed(admin.ErrorType, admin.PublicMessage);

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(clubId);
            
            if (adminCheck.Succeeded == false)
                return ResultState.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            var membership = await work.ClubMembershipRepository.FilterAsSingleAsync(x => x.ClubId == clubId && x.UserId == userId);

            if (membership is null)
                return ResultState.Failed(ResultErrorType.Validation, "User is not a member of this Club");

            if (isAdmin == membership.IsAdmin)
            {
                if (isAdmin)
                    return ResultState.Failed(ResultErrorType.Validation, "User is already an admin");
                else
                    return ResultState.Failed(ResultErrorType.Validation, "User already has a member role");
            }

            if (membership.IsAdmin)
            {
                var adminCount = await work.ClubMembershipRepository.GetCount(x=>x.ClubId == clubId && x.IsAdmin);
                
                if(adminCount == 1)
                    return ResultState.Failed(ResultErrorType.Validation, "Cannot remove the only admin from Club");
            }
            
            membership.IsAdmin = isAdmin;

            work.ClubMembershipRepository.Update(membership);
            await work.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> RemoveMemberFromClub(Guid? userId, Guid? clubId)
    {
        try
        {
            var admin = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (admin.Succeeded == false || admin.Data is null)
                return ResultState.Failed(admin.ErrorType, admin.PublicMessage);

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(clubId);
            
            if (adminCheck.Succeeded == false)
                return ResultState.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            var membership = await work.ClubMembershipRepository.FilterAsSingleAsync(x => x.ClubId == clubId && x.UserId == userId);

            if (membership is null)
                return ResultState.Failed(ResultErrorType.Validation, "User is not a member of this Club");

            if (membership.IsAdmin)
            {
                var adminCount = await work.ClubMembershipRepository.GetCount(x => x.ClubId == clubId && x.IsAdmin);

                if (adminCount == 1)
                    return ResultState.Failed(ResultErrorType.Validation, "Cannot remove the only admin from Club");
            }

            var memberCount = await work.ClubMembershipRepository.GetCount(x => x.ClubId == clubId);

            if (memberCount == 1)
                return ResultState.Failed(ResultErrorType.Validation, "Cannot leave Club when you are the only member");
            
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

    public async Task<ResultState> DeleteClub(Guid? id)
    {
        try
        {
            if (id == null)
                return ResultState.Failed(ResultErrorType.NotFound, "Club not found");

            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(id);
            
            if (adminCheck.Succeeded == false)
                return ResultState.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

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