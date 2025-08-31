using API.Services;
using Domain.Interfaces;
using Domain.Models.DTO;
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

    public ClubController(ApiResponseFactory apiResponseFactory, IClubService clubService)
    {
        _apiResponseFactory = apiResponseFactory;
        _clubService = clubService;
    }

    [HttpGet("GetClub")]
    public async Task<IActionResult> GetClub(Guid? id)
    {
        try
        {
            var result = await _clubService.GetClub(id);

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
    
    [HttpGet("GetClubs")]
    public async Task<IActionResult> GetClubs()
    {
        try
        {
            var result = await _clubService.GetClubs();

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
    
    [HttpGet("GetClubMemberships")]
    public async Task<IActionResult> GetClubMemberships(Guid? id)
    {
        try
        {
            var result = await _clubService.GetMemberships(id);

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
    
    [HttpPost("CreateClub")]
    public async Task<IActionResult> CreateClub(ClubCreateDto model)
    {
        try
        {
            var result = await _clubService.CreateClub(model);

            if (result.Succeeded)
                return Created(result.Data.ToString(), result.Data);

            return _apiResponseFactory.BadRequest(result.PublicMessage);
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

            return _apiResponseFactory.BadRequest(result.PublicMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }

    [HttpDelete("DeleteClub")]
    public async Task<IActionResult> DeleteClub(Guid? id)
    {
        try
        {
            var result = await _clubService.DeleteClub(id);

            if (result.Succeeded)
                return NoContent();

            return _apiResponseFactory.BadRequest(result.PublicMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return _apiResponseFactory.InternalServerError();
        }
    }
}