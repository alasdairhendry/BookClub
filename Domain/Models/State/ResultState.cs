using Domain.Enums;

namespace Domain.Models.State;

public class ResultState<T>
{
    public bool Succeeded { get; init; }
    public T Data { get; init; } = default!;
    public string? PublicMessage { get; init; }

    public static ResultState<T> Success(T data)
    {
        return new ResultState<T> { Succeeded = true, Data = data };
    }

    public static ResultState<T> Failed(T data, string? publicMessage)
    {
        return new ResultState<T> { Succeeded = false, Data = data, PublicMessage = publicMessage };
    }
}