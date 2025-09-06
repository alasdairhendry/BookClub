using Application.Interfaces;
using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;
using Data.Repositories;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.Dbo;

namespace Application.Services;

public class ActivityService : IActivityService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextService _httpContextService;
    private readonly IPermissionService _permissionService;

    public ActivityService(IUnitOfWork unitOfWork, IHttpContextService httpContextService, IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _httpContextService = httpContextService;
        _permissionService = permissionService;
    }

    public async Task<ResultState<ActivityDto?>> GetActivity(Guid id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<ActivityDto?>.Failed(user.ErrorType, user.PublicMessage);

            var result = await _unitOfWork.GetRepository<ActivityDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: "Club,Record");

            if (result is null)
                return ResultState<ActivityDto?>.Failed(ResultErrorType.NotFound, "Activity not found");

            var viewCheck = await _permissionService.ContextUserHasViewOfClubAsync(result.ClubId);

            if (viewCheck.Succeeded == false)
                return ResultState<ActivityDto?>.Failed(viewCheck.ErrorType, viewCheck.PublicMessage);

            ActivityDto club = ActivityDto.FromDatabaseObject(result);

            return ResultState<ActivityDto?>.Success(club);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultStateId> CreateActivity(ActivityCreateDto model, Guid clubId)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultStateId.Failed(user.ErrorType, user.PublicMessage);

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(clubId);

            if (adminCheck.Succeeded == false)
                return ResultStateId.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            var existingActivity = await _unitOfWork.GetRepository<ActivityDbo>().QueryAsSingleAsync(x =>
                x.ActualEndDate == null);

            if (existingActivity is not null)
                return ResultStateId.Failed(ResultErrorType.Validation, $"Can't have more than one activity in progress. Finish your book!");

            ActivityDbo activity = new()
            {
                State = ActivityState.Active,
                ClubId = clubId,
                RecordId = model.RecordId,
                StartDate = model.StartDate,
                TargetEndDate = model.TargetCompletionDate,
            };

            await _unitOfWork.GetRepository<ActivityDbo>().InsertAsync(activity);
            await _unitOfWork.SaveAsync();

            return ResultStateId.Success(activity.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> CompleteActivity(Guid id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var activity = await _unitOfWork.GetRepository<ActivityDbo>().GetAsync(id);

            if (activity is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Activity not found");

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(activity.ClubId);

            if (adminCheck.Succeeded == false)
                return ResultState.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            activity.ActualEndDate = DateTime.UtcNow;
            activity.State = ActivityState.Completed;

            _unitOfWork.GetRepository<ActivityDbo>().Update(activity);
            await _unitOfWork.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> CancelActivity(Guid id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var activity = await _unitOfWork.GetRepository<ActivityDbo>().GetAsync(id);

            if (activity is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Activity not found");

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(activity.ClubId);

            if (adminCheck.Succeeded == false)
                return ResultState.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            activity.ActualEndDate = DateTime.UtcNow;
            activity.State = ActivityState.Canceled;

            _unitOfWork.GetRepository<ActivityDbo>().Update(activity);
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