using Domain.Enums;

namespace Application.Models.State;

public class ResultState : ResultState<bool>
{
    public static ResultState Success()
    {
        return new ResultState { Succeeded = true, Data = true };
    }

    public static ResultState Failed()
    {
        return new ResultState { Succeeded = false, Data = false };
    }
    
    public static ResultState Failed(ResultErrorType errorType, string? publicMessage)
    {
        return new ResultState { Succeeded = false, Data = false, ErrorType = errorType, PublicMessage = publicMessage };
    }
}