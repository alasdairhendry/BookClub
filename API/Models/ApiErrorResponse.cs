namespace API.Models;

public class ApiErrorResponse
{
    public string? Message { get; init; }

    public ApiErrorResponse(string? message)
    {
        Message = message;
    }
}