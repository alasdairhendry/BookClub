using Data;
using Data.Models.Dbo;
using Domain.DataAccess;
using Domain.Interfaces;
using Domain.Models.DTO.Actions;
using Domain.Models.State;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Services;

public class InvitationService : IInvitationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextService _httpContextService;
    private readonly IPermissionService _permissionService;
    private readonly IServiceProvider _provider;

    public InvitationService(ApplicationDbContext dbContext, IHttpContextService httpContextService, IPermissionService permissionService, IServiceProvider provider)
    {
        _dbContext = dbContext;
        _httpContextService = httpContextService;
        _permissionService = permissionService;
        _provider = provider;
    }

    public async Task<ResultState> SendInvitation(InvitationCreateDto model)
    {
        try
        {
            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.PublicMessage);

            if ((await _permissionService.ContextUserIsAdminOfAsync(model.ClubId)).Succeeded == false)
                return ResultState.Failed("User does not have admin access to this resource");

            // Check user isn't already in club
            using (var scope = _provider.CreateScope())
            {
                var _accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

                if ((await _accountService.IsMemberOfClubAsync(model.ApplicationUserId, model.ClubId)).Succeeded)
                    return ResultState.Failed("User is already a member of this club");
            }

            using var work = new UnitOfWork(_dbContext);

            // Check user doesn't already have an open invitation
            if (await work.InvitationRepository.FilterAsSingleAsync(
                    x => x.DateResponded == null &&
                         x.TargetUserId == model.ApplicationUserId &&
                         x.TargetClubId == model.ClubId) != null)
                return ResultState.Failed("User has already been invited");

            InvitationDbo invitation = new InvitationDbo
            {
                DateCreated = DateTime.UtcNow,
                FromUserId = user.Data.Id,
                TargetClubId = model.ClubId,
                TargetUserId = model.ApplicationUserId
            };

            await work.InvitationRepository.InsertAsync(invitation);

            // For some reason we have to do this ?? It must somehow track the changes on the club
            // If we don't do it, the TargetClubId is filled but the navigation property doesnt link up???
            // Weird that the TargetUser and FromUser aren't an issue???
            var club = await work.ClubRepository.GetByIDAsync(model.ClubId);

            await work.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> AcceptInvitation(Guid? invitationId)
    {
        try
        {
            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            var invitation = await work.InvitationRepository.GetByIDAsync(invitationId);

            if (invitation is null)
                return ResultState.Failed("Invitation not found");

            if (user.Data.Id != invitation.TargetUserId)
                return ResultState.Failed("User cannot accept this invitation");

            // Check user isn't already in club
            using (var scope = _provider.CreateScope())
            {
                var _accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

                if ((await _accountService.IsMemberOfClubAsync(invitation.TargetUserId, invitation.TargetClubId)).Succeeded)
                    return ResultState.Failed("User is already a member of this club");
            }

            invitation.DateResponded = DateTime.UtcNow;
            invitation.Response = true;

            // Update the invitation response
            work.InvitationRepository.Update(invitation);

            ClubMembershipDbo membership = new ClubMembershipDbo
            {
                ClubId = invitation.TargetClubId,
                UserId = invitation.TargetUserId,
                IsAdmin = false
            };

            // Add the member to the club
            await work.ClubMembershipRepository.InsertAsync(membership);
            await work.SaveAsync();

            return ResultState.Success();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState> DeclineInvitation(Guid? invitationId)
    {
        try
        {
            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            var invitation = await work.InvitationRepository.GetByIDAsync(invitationId);

            if (invitation is null)
                return ResultState.Failed("Invitation not found");

            if (user.Data.Id != invitation.TargetUserId)
                return ResultState.Failed("User cannot decline this invitation");

            // Check user isn't already in club
            using (var scope = _provider.CreateScope())
            {
                var _accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

                if ((await _accountService.IsMemberOfClubAsync(invitation.TargetUserId, invitation.TargetClubId)).Succeeded)
                    return ResultState.Failed("User is already a member of this club");
            }

            invitation.DateResponded = DateTime.UtcNow;
            invitation.Response = false;

            // Update the invitation response
            work.InvitationRepository.Update(invitation);
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

public interface IInvitationService
{
    Task<ResultState> SendInvitation(InvitationCreateDto model);
    Task<ResultState> AcceptInvitation(Guid? invitationId);
    Task<ResultState> DeclineInvitation(Guid? invitationId);
}