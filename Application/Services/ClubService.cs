using Domain.Models.Dbo;
using Data.Repositories;
using Domain.Enums;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;

namespace Application.Services;

public class ClubService : IClubService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextService _httpContextService;
    private readonly IPermissionService _permissionService;

    public ClubService(IUnitOfWork unitOfWork, IHttpContextService httpContextService, IPermissionService permissionService)
    {
        _httpContextService = httpContextService;
        _permissionService = permissionService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResultState<ClubDto?>> GetClub(Guid? id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<ClubDto?>.Failed(user.ErrorType, user.PublicMessage);

            var viewCheck = await _permissionService.ContextUserHasViewOfClubAsync(id);

            if (viewCheck.Succeeded == false)
                return ResultState<ClubDto?>.Failed(viewCheck.ErrorType, viewCheck.PublicMessage);

            var result = await _unitOfWork.GetRepository<ClubDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: $"Memberships,Activities,Invitations");

            if (result is null)
                return ResultState<ClubDto?>.Failed(ResultErrorType.NotFound, "Club not found");

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

            var result = await _unitOfWork.GetRepository<ClubDbo>().QueryAsync(includeProperties: "Memberships,Activities,Invitations");

            List<ClubDto> clubs = result.Take(20).Select(x => ClubDto.FromDatabaseObject(x)).ToList();

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

            var club = await _unitOfWork.GetRepository<ClubDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: "Memberships.User");

            if (club is null)
                return ResultState<List<ClubMembershipDto>>.Failed([], ResultErrorType.NotFound, "Club not found");

            var viewCheck = await _permissionService.ContextUserHasViewOfClubAsync(id);

            if (viewCheck.Succeeded == false)
                return ResultState<List<ClubMembershipDto>>.Failed(viewCheck.ErrorType, viewCheck.PublicMessage);

            var clubMemberships = new List<ClubMembershipDto>();

            foreach (var clubMembership in club.Memberships)
            {
                var membership = await _unitOfWork.GetRepository<ClubMembershipDbo>().GetAsync(clubMembership.Id);

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

    public async Task<ResultState<List<ActivityDto>>> GetActivities(Guid? id, bool includeActive)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<List<ActivityDto>>.Failed([], user.ErrorType, user.PublicMessage);

            var club = await _unitOfWork.GetRepository<ClubDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: $"Activities.Record,Activities.Discussions");

            if (club is null)
                return ResultState<List<ActivityDto>>.Failed(ResultErrorType.NotFound, "Club not found");

            var viewCheck = await _permissionService.ContextUserHasViewOfClubAsync(id);

            if (viewCheck.Succeeded == false)
                return ResultState<List<ActivityDto>>.Failed(viewCheck.ErrorType, viewCheck.PublicMessage);

            var activities = new List<ActivityDto>();

            foreach (var activity in club.Activities)
            {
                var get = await _unitOfWork.GetRepository<ActivityDbo>().GetAsync(activity.Id);

                if (get is null)
                    continue;

                if (includeActive == false && get.State == ActivityState.Active)
                    continue;

                var dto = ActivityDto.FromDatabaseObject(get);

                activities.Add(dto);
            }

            return ResultState<List<ActivityDto>>.Success(activities);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<List<InvitationDto>>> GetInvitations(Guid? id, bool includeInactive)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<List<InvitationDto>>.Failed([], user.ErrorType, user.PublicMessage);

            var club = await _unitOfWork.GetRepository<ClubDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: $"Invitations");

            if (club is null)
                return ResultState<List<InvitationDto>>.Failed(ResultErrorType.NotFound, "Club not found");

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(id);

            if (adminCheck.Succeeded == false)
                return ResultState<List<InvitationDto>>.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            var invitations = new List<InvitationDto>();

            foreach (var invitation in club.Invitations)
            {
                var get = await _unitOfWork.GetRepository<InvitationDbo>().GetAsync(invitation.Id);

                if (get is null)
                    continue;

                if (get.Response != null && includeInactive == false)
                    continue;

                var dto = InvitationDto.FromDatabaseObject(get);

                invitations.Add(dto);
            }

            return ResultState<List<InvitationDto>>.Success(invitations);
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

            ClubDbo club = new ClubDbo
            {
                Name = model.Name,
                Motto = model.Motto,
                ImageUrl = model.ImageUrl,
                IsPrivate = model.IsPrivate,
                DateCreated = DateTime.UtcNow,
                CreatedById = user.Data.Id,
                Memberships = []
            };

            await _unitOfWork.GetRepository<ClubDbo>().InsertAsync(club);

            ClubMembershipDbo membership = new ClubMembershipDbo
            {
                ClubId = club.Id,
                UserId = user.Data.Id,
                MemberSince = DateTime.UtcNow,
                IsAdmin = true
            };

            await _unitOfWork.GetRepository<ClubMembershipDbo>().InsertAsync(membership);
            await _unitOfWork.SaveAsync();

            return ResultStateId.Success(club.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> UpdateClub(ClubUpdateDto model, Guid clubId)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(clubId);

            if (adminCheck.Succeeded == false)
                return ResultState.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            ClubDbo? club = await _unitOfWork.GetRepository<ClubDbo>().GetAsync(clubId);

            if (club is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Club not found");

            if (string.IsNullOrWhiteSpace(model.Name))
                return ResultState.Failed(ResultErrorType.Validation, "Club must have a name");

            club.Name = model.Name;
            club.Motto = model.Motto;
            club.ImageUrl = model.ImageUrl;
            club.IsPrivate = model.IsPrivate;

            _unitOfWork.GetRepository<ClubDbo>().Update(club);
            await _unitOfWork.SaveAsync();

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

            var membership = await _unitOfWork.GetRepository<ClubMembershipDbo>().QueryAsSingleAsync(x => x.ClubId == clubId && x.UserId == userId);

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
                var adminCount = await _unitOfWork.GetRepository<ClubMembershipDbo>().GetCount(x => x.ClubId == clubId && x.IsAdmin);

                if (adminCount == 1)
                    return ResultState.Failed(ResultErrorType.Validation, "Cannot remove the only admin from Club");
            }

            membership.IsAdmin = isAdmin;

            _unitOfWork.GetRepository<ClubMembershipDbo>().Update(membership);
            await _unitOfWork.SaveAsync();

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

            var membership = await _unitOfWork.GetRepository<ClubMembershipDbo>().QueryAsSingleAsync(x => x.ClubId == clubId && x.UserId == userId);

            if (membership is null)
                return ResultState.Failed(ResultErrorType.Validation, "User is not a member of this Club");

            if (membership.IsAdmin)
            {
                var adminCount = await _unitOfWork.GetRepository<ClubMembershipDbo>().GetCount(x => x.ClubId == clubId && x.IsAdmin);

                if (adminCount == 1)
                    return ResultState.Failed(ResultErrorType.Validation, "Cannot remove the only admin from Club");
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

            await _unitOfWork.GetRepository<ClubDbo>().DeleteAsync(id);
            await _unitOfWork.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}