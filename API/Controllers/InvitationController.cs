using API.Services;
using Domain.Interfaces;
using Domain.Models.DTO.Actions;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class InvitationController : ControllerBase
{
    private readonly ApiResponseFactory _apiResponseFactory;
    private readonly IInvitationService _invitationService;

    public InvitationController(ApiResponseFactory apiResponseFactory, IInvitationService invitationService)
    {
        _apiResponseFactory = apiResponseFactory;
        _invitationService = invitationService;
    }

    [HttpPost("SendInvitation")]
    public async Task<IActionResult> SendInvitation(InvitationCreateDto model)
    {
        try
        {
            var result = await _invitationService.SendInvitation(model);

            if (result.Succeeded)
                return Ok(result.Data);

            return _apiResponseFactory.BadRequest(result.PublicMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }

    [HttpPatch("AcceptInvitation")]
    public async Task<IActionResult> AcceptInvitation(Guid? invitationId)
    {
        try
        {
            var result = await _invitationService.AcceptInvitation(invitationId);

            if (result.Succeeded)
                return Ok(result.Data);

            return _apiResponseFactory.BadRequest(result.PublicMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }
    
    [HttpPatch("DeclineInvitation")]
    public async Task<IActionResult> DeclineInvitation(Guid? invitationId)
    {
        try
        {
            var result = await _invitationService.DeclineInvitation(invitationId);

            if (result.Succeeded)
                return Ok(result.Data);

            return _apiResponseFactory.BadRequest(result.PublicMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }
    
}