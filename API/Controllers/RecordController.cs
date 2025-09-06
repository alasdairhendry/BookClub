using API.Services;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RecordController : ControllerBase
{
    private readonly ApiResponseFactory _apiResponseFactory;
    private readonly IRecordService _recordService;

    public RecordController(ApiResponseFactory apiResponseFactory, IRecordService recordService)
    {
        _apiResponseFactory = apiResponseFactory;
        _recordService = recordService;
    }
    
    [HttpGet("GetSuggestion")]
    public async Task<IActionResult> GetSuggestion(string request)
    {
        try
        {
            var result = await _recordService.GetSuggestionResultAsync(request);;

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

    [HttpGet("GetRecord")]
    public async Task<IActionResult> GetRecord(Guid id)
    {
        try
        {
            var result = await _recordService.GetRecordAsync(id);;

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
    
    [HttpPost("Search")]
    public async Task<IActionResult> Search(SearchRequestDto awd)
    {
        try
        {
            var result = await _recordService.SearchAsync(awd);

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