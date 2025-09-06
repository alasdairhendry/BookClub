using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;

namespace Application.Interfaces;

public interface IActivityService
{
    Task<ResultState<ActivityDto?>> GetActivity(Guid id);
    Task<ResultStateId> CreateActivity(ActivityCreateDto model, Guid clubId);
    Task<ResultState> CompleteActivity(Guid id);
    Task<ResultState> CancelActivity(Guid id);
}