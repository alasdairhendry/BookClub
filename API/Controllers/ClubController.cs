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

    [HttpGet("GetClub")]
    public async Task<IActionResult> GetClub(Guid id)
    {
        try
        {
            var result = await _clubService.GetClub(id);

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

    [HttpGet("GetClubMemberships")]
    public async Task<IActionResult> GetClubMemberships(Guid id)
    {
        try
        {
            var result = await _clubService.GetMemberships(id);

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

    [HttpPatch("UpdateClub")]
    public async Task<IActionResult> UpdateClub(ClubUpdateDto model)
    {
        try
        {
            var result = await _clubService.UpdateClub(model);

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

    [HttpPatch("UpdateMemberRole")]
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

    [HttpDelete("RemoveMemberFromClub")]
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

    [HttpDelete("DeleteClub")]
    public async Task<IActionResult> DeleteClub(Guid id)
    {
        try
        {
            var result = await _clubService.DeleteClub(id);

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
}