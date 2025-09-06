using API.Services;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DiscussionController : ControllerBase
{
    private readonly ApiResponseFactory _apiResponseFactory;
    private readonly IDiscussionService _discussionService;

    public DiscussionController(ApiResponseFactory apiResponseFactory, IDiscussionService discussionService)
    {
        _apiResponseFactory = apiResponseFactory;
        _discussionService = discussionService;
    }

    [HttpGet("{discussionId:guid}")]
    public async Task<IActionResult> GetDiscussion(Guid discussionId)
    {
        try
        {
            var result = await _discussionService.GetDiscussion(discussionId);

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
    
    [HttpPatch("{discussionId:guid}/Close")]
    public async Task<IActionResult> CloseDiscussion(Guid discussionId)
    {
        try
        {
            var result = await _discussionService.CloseDiscussion(discussionId);

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
    
    [HttpDelete("{discussionId:guid}/Delete")]
    public async Task<IActionResult> DeleteDiscussion(Guid discussionId)
    {
        try
        {
            var result = await _discussionService.DeleteDiscussion(discussionId);

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
    
    [HttpGet("{discussionId:guid}/GetComments")]
    public async Task<IActionResult> GetComments(Guid discussionId)
    {
        try
        {
            var result = await _discussionService.GetComments(discussionId);

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

    [HttpPost("{discussionId:guid}/CreateComment")]
    public async Task<IActionResult> CreateComment(CommentCreateDto model, Guid discussionId)
    {
        try
        {
            var result = await _discussionService.CreateComment(model, discussionId);

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