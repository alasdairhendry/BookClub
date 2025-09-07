using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;

namespace Application.Interfaces;

public interface IDiscussionService
{
    Task<ResultState<DiscussionDto?>> GetDiscussion(Guid id);
    Task<ResultState<List<DiscussionDto>>> GetDiscussions(Guid activityId);
    Task<ResultState<List<CommentDto>>> GetComments(Guid discussionId);
    Task<ResultStateId> CreateDiscussion(DiscussionCreateDto model, Guid activityId);
    Task<ResultStateId> CreateComment(CommentCreateDto model, Guid discussionId);
    Task<ResultState> CloseDiscussion(Guid id);
    Task<ResultState> DeleteDiscussion(Guid id);
    Task<ResultState> SoftDeleteComment(Guid id);
}