namespace Application.Models.Dto.Objects;

public class EntityIdDto
{
    public Guid? Id { get; set; }

    public EntityIdDto(Guid? id)
    {
        Id = id;
    }
}