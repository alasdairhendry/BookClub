using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public class ApiResponseFactory
{
    public ObjectResult InternalServerError()
    {
        return new ObjectResult(new ApiErrorResponse("An unexpected error has occurred"))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
    
    public BadRequestObjectResult BadRequest(string? message)
    {
        return new BadRequestObjectResult(new ApiErrorResponse(message));
    }
}