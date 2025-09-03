namespace IntegrationTests.Models.State;

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
    
    public static ResultState Failed(string? publicMessage)
    {
        return new ResultState { Succeeded = false, Data = false, PublicMessage = publicMessage };
    }
}