using API.Services;
using Domain.Interfaces;
using Domain.Models.DTO;
using Domain.Models.DTO.Actions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApiResponseFactory _apiResponseFactory;
    private readonly IUserService _userService;

    public AuthController(ApiResponseFactory apiResponseFactory, IUserService userService)
    {
        _apiResponseFactory = apiResponseFactory;
        _userService = userService;
    }

    [HttpGet("IntegrationTest")]
    public async Task<IActionResult> IntegrationTest()
    {
        await Task.Delay(1);
        return Ok("Hello world");
    }
    
    [HttpPost("Register")]
    public async Task<IActionResult> Register(UserRegistrationDto model)
    {
        try
        {
            var result = await _userService.Register(model);

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
    
    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLoginDto model)
    {
        try
        {
            var result = await _userService.Login(model);
            
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
    
    [HttpPost("GetToken")]
    public async Task<IActionResult> GetToken(UserLoginDto model)
    {
        try
        {
            var result = await _userService.GetToken(model);
            
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