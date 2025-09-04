using Application.Models.State;

namespace Application.Interfaces;

public interface IPermissionService
{
    Task<ResultState> ContextUserHasViewOfClubAsync(Guid? clubId);
    Task<ResultState> ContextUserIsMemberOfClubAsync(Guid? clubId);
    Task<ResultState> ContextUserIsAdminOfClubAsync(Guid? clubId);
    Task<ResultState> UserHasViewOfClubAsync(Guid? userId, Guid? clubId);
    Task<ResultState> UserIsMemberOfClubAsync(Guid? userId, Guid? clubId);
    Task<ResultState> UserIsAdminOfClubAsync(Guid? userId, Guid? clubId);
}