using Domain.Enums;
using Application.Models.Dto.Objects;

namespace Application.Models.State;

public class ResultStateId : ResultState<EntityIdDto?>
{
    public static ResultStateId Success(Guid id)
    {
        return new ResultStateId { Succeeded = true, Data = new EntityIdDto(id) };
    }

    public static ResultStateId Failed()
    {
        return new ResultStateId { Succeeded = false, Data = new EntityIdDto(null) };
    }
    
    public static ResultStateId Failed(ResultErrorType errorType, string? publicMessage)
    {
        return new ResultStateId { Succeeded = false, Data = new EntityIdDto(null), ErrorType = errorType, PublicMessage = publicMessage };
    }
}