using Domain.Interfaces;
using Domain.Models.State;

namespace Domain.Services;

public class PermissionService : IPermissionService
{
    public async Task<ResultState> ContextUserHasAccessToAsync(Guid? resource)
    {
        // TODO - Implement
        await Task.Delay(1);
        return ResultState.Success();
    }

    public async Task<ResultState> ContextUserIsAdminOfAsync(Guid? resource)
    {
        // TODO - Implement
        await Task.Delay(1);
        return ResultState.Success();
    }
}