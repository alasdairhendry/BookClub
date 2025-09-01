using Data;
using Data.Models.Dbo;
using Domain.DataAccess;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models.DTO.Actions;
using Domain.Models.DTO.Objects;
using Domain.Models.State;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Services;

public class InvitationService : IInvitationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextService _httpContextService;
    private readonly IPermissionService _permissionService;
    private readonly IAccountService _accountService;

    public InvitationService(ApplicationDbContext dbContext, IHttpContextService httpContextService,
        IPermissionService permissionService, IServiceProvider provider, IAccountService accountService)
    {
        _dbContext = dbContext;
        _httpContextService = httpContextService;
        _permissionService = permissionService;
        _accountService = accountService;
    }

    public async Task<ResultStateId> SendInvitation(InvitationCreateDto model)
    {
        try
        {
            var user = await _httpContextService.ContextUserIsActiveAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultStateId.Failed(user.ErrorType, user.PublicMessage);

            if ((await _permissionService.ContextUserIsAdminOfAsync(model.ClubId)).Succeeded == false)
                return ResultStateId.Failed(ResultErrorType.Unauthorised, "User does not have admin access to this resource");

            using var work = new UnitOfWork(_dbContext);

            // Check club exists
            if (await work.ClubRepository.GetByIDAsync(model.ClubId) == null)
                return ResultStateId.Failed(ResultErrorType.NotFound, "Club does not exist");

            // Check user isn't already in club
            if ((await _accountService.IsMemberOfClubAsync(model.ApplicationUserId, model.ClubId)).Succeeded)
                return ResultStateId.Failed(ResultErrorType.Validation, "User is already a member of this club");

            // Check user doesn't already have an open invitation
            if (await work.InvitationRepository.FilterAsSingleAsync(
                    x => x.DateResponded == null &&
                         x.TargetUserId == model.ApplicationUserId &&
                         x.TargetClubId == model.ClubId) != null)
                return ResultStateId.Failed(ResultErrorType.Validation, "User has already been invited");

            InvitationDbo createdInvitation = new InvitationDbo
            {
                DateCreated = DateTime.UtcNow,
                FromUserId = user.Data.Id,
                TargetClubId = model.ClubId,
                TargetUserId = model.ApplicationUserId
            };

            await work.InvitationRepository.InsertAsync(createdInvitation);
            await work.SaveAsync();

            return ResultStateId.Success(createdInvitation.Id);
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
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            var invitation = await work.InvitationRepository.GetByIDAsync(invitationId);

            if (invitation is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Invitation not found");

            if (user.Data.Id != invitation.TargetUserId)
                return ResultState.Failed(ResultErrorType.Validation, "User cannot accept this invitation");

            // Check user isn't already in club
            if ((await _accountService.IsMemberOfClubAsync(invitation.TargetUserId, invitation.TargetClubId)).Succeeded)
                return ResultState.Failed(ResultErrorType.Validation, "User is already a member of this club");

            invitation.DateResponded = DateTime.UtcNow;
            invitation.Response = true;

            // Update the invitation response
            work.InvitationRepository.Update(invitation);

            ClubMembershipDbo membership = new ClubMembershipDbo
            {
                ClubId = invitation.TargetClubId,
                UserId = invitation.TargetUserId,
                MemberSince = DateTime.UtcNow,
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
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            using var work = new UnitOfWork(_dbContext);

            var invitation = await work.InvitationRepository.GetByIDAsync(invitationId);

            if (invitation is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Invitation not found");

            if (user.Data.Id != invitation.TargetUserId)
                return ResultState.Failed(ResultErrorType.Validation, "User cannot decline this invitation");

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