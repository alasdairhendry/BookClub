using API.Services;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly ApiResponseFactory _apiResponseFactory;
    private readonly IAccountService _accountService;

    public AccountController(ApiResponseFactory apiResponseFactory, IAccountService accountService)
    {
        _apiResponseFactory = apiResponseFactory;
        _accountService = accountService;
    }

    [HttpGet("GetUserDetails/{userId:guid}")]
    public async Task<IActionResult> GetUserDetails(Guid userId)
    {
        try
        {
            var result = await _accountService.GetUserDetailsAsync(userId);

            if (result.Succeeded)
                return Ok(result.Data);

            return _apiResponseFactory.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }

    [HttpGet("GetUserClubMemberships/{userId:guid}")]
    public async Task<IActionResult> GetUserClubMemberships(Guid userId)
    {
        try
        {
            var result = await _accountService.GetUserClubMembershipsAsync(userId);

            if (result.Succeeded)
                return Ok(result.Data);

            return _apiResponseFactory.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }
    
    [HttpGet("GetUserClubInvitations/{userId:guid}")] 
    public async Task<IActionResult> GetUserClubInvitations(Guid userId, bool activeOnly = false)
    {
        try
        {
            var result = await _accountService.GetUserClubInvitationsAsync(userId, activeOnly);

            if (result.Succeeded)
                return Ok(result.Data);

            return _apiResponseFactory.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }

    [HttpDelete("LeaveClub/{clubId:guid}")]
    public async Task<IActionResult> LeaveClub(Guid clubId)
    {
        try
        {
            var result = await _accountService.LeaveClub(clubId);

            if (result.Succeeded)
                return Ok(result.Data);

            return _apiResponseFactory.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }
}