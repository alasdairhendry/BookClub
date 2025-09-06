using API.Services;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ApiResponseFactory _apiResponseFactory;
    private readonly IDiscussionService _discussionService;

    public CommentController(ApiResponseFactory apiResponseFactory, IDiscussionService discussionService)
    {
        _apiResponseFactory = apiResponseFactory;
        _discussionService = discussionService;
    }

    [HttpPatch("{commentId:guid}/SoftDelete")]
    public async Task<IActionResult> SoftDeleteComment(Guid commentId)
    {
        try
        {
            var result = await _discussionService.SoftDeleteComment(commentId);

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