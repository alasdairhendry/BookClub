using Domain.Models.Dbo;
using Data.Repositories;
using Domain.Enums;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Application.Models.State;

namespace Application.Services;

public class InvitationService : IInvitationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextService _httpContextService;
    private readonly IPermissionService _permissionService;
    private readonly IAccountService _accountService;

    public InvitationService(IUnitOfWork unitOfWork, IHttpContextService httpContextService,
        IPermissionService permissionService, IServiceProvider provider, IAccountService accountService)
    {
        _unitOfWork = unitOfWork;
        _httpContextService = httpContextService;
        _permissionService = permissionService;
        _accountService = accountService;
    }

    public async Task<ResultStateId> CreateInvitation(InvitationCreateDto model)
    {
        try
        {
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultStateId.Failed(user.ErrorType, user.PublicMessage);

            var adminCheck = await _permissionService.ContextUserIsAdminOfClubAsync(model.ClubId);
            
            if (adminCheck.Succeeded == false)
                return ResultStateId.Failed(adminCheck.ErrorType, adminCheck.PublicMessage);

            // Check club exists
            if (await _unitOfWork.GetRepository<ClubDbo>().GetAsync(model.ClubId) == null)
                return ResultStateId.Failed(ResultErrorType.NotFound, "Club does not exist");

            // Check user isn't already in club
            if ((await _accountService.IsMemberOfClubAsync(model.ApplicationUserId, model.ClubId)).Succeeded)
                return ResultStateId.Failed(ResultErrorType.Validation, "User is already a member of this Club");

            // Check user doesn't already have an open invitation
            if (await _unitOfWork.GetRepository<InvitationDbo>().QueryAsSingleAsync(
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

            await _unitOfWork.GetRepository<InvitationDbo>().InsertAsync(createdInvitation);
            await _unitOfWork.SaveAsync();

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
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var invitation = await _unitOfWork.GetRepository<InvitationDbo>().GetAsync(invitationId);

            if (invitation is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Invitation not found");

            if (user.Data.Id != invitation.TargetUserId)
                return ResultState.Failed(ResultErrorType.Validation, "User cannot accept this invitation");

            // Check user isn't already in club
            if ((await _accountService.IsMemberOfClubAsync(invitation.TargetUserId, invitation.TargetClubId)).Succeeded)
                return ResultState.Failed(ResultErrorType.Validation, "User is already a member of this Club");

            if(invitation.Response != null)
                return ResultState.Failed(ResultErrorType.Validation, "Invitation has already been responded to");
            
            invitation.DateResponded = DateTime.UtcNow;
            invitation.Response = true;

            // Update the invitation response
            _unitOfWork.GetRepository<InvitationDbo>().Update(invitation);

            ClubMembershipDbo membership = new ClubMembershipDbo
            {
                ClubId = invitation.TargetClubId,
                UserId = invitation.TargetUserId,
                MemberSince = DateTime.UtcNow,
                IsAdmin = false
            };

            // Add the member to the club
            await _unitOfWork.GetRepository<ClubMembershipDbo>().InsertAsync(membership);
            await _unitOfWork.SaveAsync();

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
            var user = await _httpContextService.ContextApplicationUserIsEnabledAsync();

            if (user.Succeeded == false || user.Data is null)
                return ResultState.Failed(user.ErrorType, user.PublicMessage);

            var invitation = await _unitOfWork.GetRepository<InvitationDbo>().GetAsync(invitationId);

            if (invitation is null)
                return ResultState.Failed(ResultErrorType.NotFound, "Invitation not found");

            if (user.Data.Id != invitation.TargetUserId)
                return ResultState.Failed(ResultErrorType.Validation, "User cannot decline this invitation");

            if(invitation.Response != null)
                return ResultState.Failed(ResultErrorType.Validation, "Invitation has already been responded to");
            
            invitation.DateResponded = DateTime.UtcNow;
            invitation.Response = false;

            // Update the invitation response
            _unitOfWork.GetRepository<InvitationDbo>().Update(invitation);
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