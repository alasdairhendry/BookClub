using API.Services;
using Domain.Interfaces;
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
    public async Task<IActionResult> GetUserDetails(Guid? id)
    {
        try
        {
            var result = await _accountService.GetUserDetails(id);

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

    [HttpGet("GetUserClubMemberships")]
    public async Task<IActionResult> GetUserClubMemberships(Guid? id)
    {
        try
        {
            var result = await _accountService.GetUserClubMemberships(id);

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