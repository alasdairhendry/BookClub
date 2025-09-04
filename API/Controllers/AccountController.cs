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

    [HttpGet("GetUserDetails")]
    public async Task<IActionResult> GetUserDetails(Guid id)
    {
        try
        {
            var result = await _accountService.GetUserDetailsAsync(id);

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

    [HttpGet("GetUserClubMemberships")]
    public async Task<IActionResult> GetUserClubMemberships(Guid id)
    {
        try
        {
            var result = await _accountService.GetUserClubMembershipsAsync(id);

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
    
    [HttpGet("GetUserClubInvitations")] 
    public async Task<IActionResult> GetUserClubInvitations(Guid id, bool activeOnly = false)
    {
        try
        {
            var result = await _accountService.GetUserClubInvitationsAsync(id, activeOnly);

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

    [HttpDelete("LeaveClub")]
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