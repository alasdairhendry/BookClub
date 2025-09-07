using API.Services;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly ApiResponseFactory _apiResponseFactory;
    private readonly IActivityService _activityService;
    private readonly IDiscussionService _discussionService;

    public ActivityController(ApiResponseFactory apiResponseFactory, IActivityService activityService, IDiscussionService discussionService)
    {
        _apiResponseFactory = apiResponseFactory;
        _activityService = activityService;
        _discussionService = discussionService;
    }

    [HttpGet("{activityId:guid}")]
    public async Task<IActionResult> GetActivity(Guid activityId)
    {
        try
        {
            var result = await _activityService.GetActivity(activityId);

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

    [HttpPatch("{activityId:guid}/CompleteActivity")]
    public async Task<IActionResult> CompleteActivity(Guid activityId)
    {
        try
        {
            var result = await _activityService.CompleteActivity(activityId);

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

    [HttpPatch("{activityId:guid}/CancelActivity")]
    public async Task<IActionResult> CancelActivity(Guid activityId)
    {
        try
        {
            var result = await _activityService.CancelActivity(activityId);

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
    
    [HttpPost("{activityId:guid}/CreateDiscussion")]
    public async Task<IActionResult> CreateDiscussion(DiscussionCreateDto model, Guid activityId)
    {
        try
        {
            var result = await _discussionService.CreateDiscussion(model, activityId);

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
    
    [HttpGet("{activityId:guid}/GetDiscussions")]
    public async Task<IActionResult> GetDiscussions(Guid activityId)
    {
        try
        {
            var result = await _discussionService.GetDiscussions(activityId);

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