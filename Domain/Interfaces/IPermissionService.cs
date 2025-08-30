using Domain.Models.State;

namespace Domain.Interfaces;

public interface IPermissionService
{
    Task<ResultState> ContextUserHasAccessToAsync(Guid? resource);
    
    Task<ResultState> ContextUserIsAdminOfAsync(Guid? resource);
}