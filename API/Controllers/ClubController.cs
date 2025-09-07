using API.Services;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ClubController : ControllerBase
{
    private readonly ApiResponseFactory _apiResponseFactory;
    private readonly IClubService _clubService;
    private readonly IActivityService _activityService;

    public ClubController(ApiResponseFactory apiResponseFactory, IClubService clubService, IActivityService activityService)
    {
        _apiResponseFactory = apiResponseFactory;
        _clubService = clubService;
        _activityService = activityService;
    }

    [HttpGet("{clubId:guid}")]
    public async Task<IActionResult> GetClub(Guid clubId)
    {
        try
        {
            var result = await _clubService.GetClub(clubId);

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

    [HttpGet("GetClubs")]
    public async Task<IActionResult> GetClubs()
    {
        try
        {
            var result = await _clubService.GetClubs();

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

    [HttpPost("CreateClub")]
    public async Task<IActionResult> CreateClub(ClubCreateDto model)
    {
        try
        {
            var result = await _clubService.CreateClub(model);

            if (result.Succeeded)
                return Created(result.Data!.Id.ToString(), result.Data);

            return _apiResponseFactory.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }

    [HttpPatch("{clubId:guid}/UpdateClub")]
    public async Task<IActionResult> UpdateClub(ClubUpdateDto model, Guid clubId)
    {
        try
        {
            var result = await _clubService.UpdateClub(model, clubId);

            if (result.Succeeded)
                return NoContent();

            return _apiResponseFactory.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }

    [HttpDelete("{clubId:guid}/DeleteClub")]
    public async Task<IActionResult> DeleteClub(Guid clubId)
    {
        try
        {
            var result = await _clubService.DeleteClub(clubId);

            if (result.Succeeded)
                return NoContent();

            return _apiResponseFactory.FromResult(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }

    [HttpPatch("{clubId:guid}/UpdateMemberRole")]
    public async Task<IActionResult> UpdateMemberRole(Guid userId, Guid clubId, bool isAdmin)
    {
        try
        {
            var result = await _clubService.UpdateMemberRole(userId, clubId, isAdmin);

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

    [HttpDelete("{clubId:guid}/RemoveMemberFromClub")]
    public async Task<IActionResult> RemoveMemberFromClub(Guid userId, Guid clubId)
    {
        try
        {
            var result = await _clubService.RemoveMemberFromClub(userId, clubId);

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

    [HttpPost("{clubId:guid}/CreateActivity")]
    public async Task<IActionResult> CreateActivity(ActivityCreateDto model, Guid clubId)
    {
        try
        {
            var result = await _activityService.CreateActivity(model, clubId);

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

    [HttpGet("{clubId:guid}/GetMemberships")]
    public async Task<IActionResult> GetMemberships(Guid clubId)
    {
        try
        {
            var result = await _clubService.GetMemberships(clubId);

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

    [HttpGet("{clubId:guid}/GetActivities")]
    public async Task<IActionResult> GetActivities(Guid clubId, bool includeActive)
    {
        try
        {
            var result = await _clubService.GetActivities(clubId, includeActive);

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

    [HttpGet("{clubId:guid}/GetInvitations")]
    public async Task<IActionResult> GetInvitations(Guid clubId, bool includeInactive)
    {
        try
        {
            var result = await _clubService.GetInvitations(clubId, includeInactive);

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