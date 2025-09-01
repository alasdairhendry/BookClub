using Domain.Enums;

namespace Domain.Models.State;

public class ResultState<T>
{
    public bool Succeeded { get; init; }
    public T Data { get; init; } = default!;
    public string? PublicMessage { get; init; }
    public ResultErrorType ErrorType { get; init; } 

    public static ResultState<T> Success(T data)
    {
        return new ResultState<T> { Succeeded = true, Data = data, ErrorType = ResultErrorType.None };
    }

    public static ResultState<T> Failed(T data, ResultErrorType errorType , string? publicMessage)
    {
        return new ResultState<T> { Succeeded = false, Data = data, ErrorType = errorType, PublicMessage = publicMessage };
    }
}