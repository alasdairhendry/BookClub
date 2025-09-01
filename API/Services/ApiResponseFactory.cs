using API.Models;
using Domain.Enums;
using Domain.Models.DTO.Objects;
using Domain.Models.State;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public class ApiResponseFactory
{
    public ObjectResult InternalServerError()
    {
        return new ObjectResult(new ApiErrorResponse("An error has occurred"))
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }

    public BadRequestObjectResult BadRequest(string? message)
    {
        return new BadRequestObjectResult(new ApiErrorResponse(message));
    }

    public IActionResult FromResult<T>(ResultState<T> result)
    {
        switch (result.ErrorType)
        {
            case ResultErrorType.Conflict:
                return new ConflictObjectResult(new ApiErrorResponse(result.PublicMessage));
            case ResultErrorType.Validation:
                return new BadRequestObjectResult(new ApiErrorResponse(result.PublicMessage));
            case ResultErrorType.NotFound:
                return new NotFoundObjectResult(new ApiErrorResponse(result.PublicMessage));
            case ResultErrorType.Unauthorised:
                return new UnauthorizedObjectResult(new ApiErrorResponse(result.PublicMessage));
            default:
                return InternalServerError();
        }
    }
}