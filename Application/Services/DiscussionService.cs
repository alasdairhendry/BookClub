using Application.Interfaces;
using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;
using Data.Repositories;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.Dbo;

namespace Application.Services;

public class DiscussionService : IDiscussionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextService _httpContextService;
    private readonly IPermissionService _permissionService;

    public DiscussionService(IUnitOfWork unitOfWork, IHttpContextService httpContextService, IPermissionService permissionService)
    {
        _unitOfWork = unitOfWork;
        _httpContextService = httpContextService;
        _permissionService = permissionService;
    }

    public async Task<ResultState<DiscussionDto?>> GetDiscussion(Guid id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<DiscussionDto?>.Failed(user.ErrorType, user.PublicMessage);

            var result = await _unitOfWork.GetRepository<DiscussionDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: "Activity.Club");

            if (result is null)
                return ResultState<DiscussionDto?>.Failed(ResultErrorType.NotFound, "Discussion not found");

            var viewCheck = await _permissionService.ContextUserHasViewOfClubAsync(result.Activity.ClubId);

            if (viewCheck.Succeeded == false)
                return ResultState<DiscussionDto?>.Failed(viewCheck.ErrorType, viewCheck.PublicMessage);

            DiscussionDto discussion = DiscussionDto.FromDatabaseObject(result);

            return ResultState<DiscussionDto?>.Success(discussion);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<List<DiscussionDto>>> GetDiscussions(Guid activityId)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<List<DiscussionDto>>.Failed([], user.ErrorType, user.PublicMessage);

            var activity = await _unitOfWork.GetRepository<ActivityDbo>().QueryAsSingleAsync(x => x.Id == activityId, includeProperties: "Club");

            if (activity is null)
                return ResultState<List<DiscussionDto>>.Failed([], ResultErrorType.NotFound, "Activity not found");

            var viewCheck = await _permissionService.ContextUserHasViewOfClubAsync(activity.ClubId);

            if (viewCheck.Succeeded == false)
                return ResultState<List<DiscussionDto>>.Failed([], viewCheck.ErrorType, viewCheck.PublicMessage);

            var discussions = await _unitOfWork.GetRepository<DiscussionDbo>().QueryAsync(x => x.ActivityId == activityId);

            var result = discussions.Select(DiscussionDto.FromDatabaseObject).ToList();

            return ResultState<List<DiscussionDto>>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<List<CommentDto>>> GetComments(Guid discussionId)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState<List<CommentDto>>.Failed([], user.ErrorType, user.PublicMessage);

            var discussion = await _unitOfWork.GetRepository<DiscussionDbo>().QueryAsSingleAsync(x => x.Id == discussionId, includeProperties: "Activity");

            if (discussion is null)
                return ResultState<List<CommentDto>>.Failed([], ResultErrorType.NotFound, "Discussion not found");

            var viewCheck = await _permissionService.ContextUserHasViewOfClubAsync(discussion.Activity.ClubId);

            if (viewCheck.Succeeded == false)
                return ResultState<List<CommentDto>>.Failed([], viewCheck.ErrorType, viewCheck.PublicMessage);

            var comments = await _unitOfWork.GetRepository<CommentDbo>()
                .QueryAsync(x => x.DiscussionId == discussionId,
                    orderBy: dbos => dbos.OrderBy(x => x.DateCreated));

            var result = comments.Select(CommentDto.FromDatabaseObject).ToList();

            return ResultState<List<CommentDto>>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<ResultStateId> CreateDiscussion(DiscussionCreateDto model, Guid activityId)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultStateId.Failed(user.ErrorType, user.PublicMessage);

            var activity = await _unitOfWork.GetRepository<ActivityDbo>().QueryAsSingleAsync(x => x.Id == activityId);

            if (activity is null)
                return ResultStateId.Failed(ResultErrorType.NotFound, "Activity not found");

            var memberCheck = await _permissionService.ContextUserIsMemberOfClubAsync(activity.ClubId);

            if (memberCheck.Succeeded == false)
                return ResultStateId.Failed(memberCheck.ErrorType, memberCheck.PublicMessage);

            var discussion = new DiscussionDbo()
            {
                ActivityId = activityId,
                UserId = user.Data.Id,
                Username = user.Data.UserName!,
                Title = model.Title,
                Description = model.Description,
                DateCreated = DateTime.Now,
                IsClosed = false,
                Page = model.Page,
            };

            await _unitOfWork.GetRepository<DiscussionDbo>().InsertAsync(discussion);
            await _unitOfWork.SaveAsync();

            return ResultStateId.Success(discussion.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultStateId> CreateComment(CommentCreateDto model, Guid discussionId)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultStateId.Failed(user.ErrorType, user.PublicMessage);

            var discussion = await _unitOfWork.GetRepository<DiscussionDbo>().QueryAsSingleAsync(x => x.Id == discussionId, includeProperties: "Activity");

            if (discussion is null)
                return ResultStateId.Failed(ResultErrorType.NotFound, "Discussion not found");

            var memberCheck = await _permissionService.ContextUserIsMemberOfClubAsync(discussion.Activity.ClubId);

            if (memberCheck.Succeeded == false)
                return ResultStateId.Failed(memberCheck.ErrorType, memberCheck.PublicMessage);

            var comment = new CommentDbo
            {
                DiscussionId = discussion.Id,
                UserId = user.Data.Id,
                Username = user.Data.UserName,
                ParentId = model.ParentId,
                DateCreated = DateTime.Now,
                Message = model.Message,
            };

            await _unitOfWork.GetRepository<CommentDbo>().InsertAsync(comment);
            await _unitOfWork.SaveAsync();

            return ResultStateId.Success(comment.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> CloseDiscussion(Guid id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var discussion = await _unitOfWork.GetRepository<DiscussionDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: "Activity");

            if (discussion is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Discussion not found");

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(discussion.Activity.ClubId);
            var userIsOwner = discussion.UserId == user.Data.Id;

            if (userIsOwner == false && adminCheck.Succeeded == false)
            {
                return ResultState.Failed(ResultErrorType.Unauthorised, "You do not have permission to close this discussion");
            }

            discussion.IsClosed = true;
            
            _unitOfWork.GetRepository<DiscussionDbo>().Update(discussion);
            await _unitOfWork.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> DeleteDiscussion(Guid id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var discussion = await _unitOfWork.GetRepository<DiscussionDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: "Activity");

            if (discussion is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Discussion not found");

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(discussion.Activity.ClubId);
            var userIsOwner = discussion.UserId == user.Data.Id;

            if (userIsOwner == false && adminCheck.Succeeded == false)
            {
                return ResultState.Failed(ResultErrorType.Unauthorised, "You do not have permission to delete this discussion");
            }

            _unitOfWork.GetRepository<DiscussionDbo>().Delete(discussion);
            await _unitOfWork.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> SoftDeleteComment(Guid id)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var comment = await _unitOfWork.GetRepository<CommentDbo>().QueryAsSingleAsync(x => x.Id == id, includeProperties: "Discussion.Activity");

            if (comment is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Comment not found");

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(comment.Discussion.Activity.ClubId);
            var userIsOwner = comment.UserId == user.Data.Id;

            if (userIsOwner == false && adminCheck.Succeeded == false)
            {
                return ResultState.Failed(ResultErrorType.Unauthorised, "You do not have permission to delete this comment");
            }

            comment.SoftDelete = true;
            
            _unitOfWork.GetRepository<CommentDbo>().Update(comment);
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