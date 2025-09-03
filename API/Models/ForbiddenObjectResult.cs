using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API.Models;

[DefaultStatusCode(DefaultStatusCode)]
public class ForbiddenObjectResult : ObjectResult
{
    private const int DefaultStatusCode = StatusCodes.Status403Forbidden;

    /// <summary>
    /// Creates a new <see cref="UnauthorizedObjectResult"/> instance.
    /// </summary>
    public ForbiddenObjectResult([ActionResultObjectValue] object? value) : base(value)
    {
        StatusCode = DefaultStatusCode;
    }
}