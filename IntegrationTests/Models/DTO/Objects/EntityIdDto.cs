namespace IntegrationTests.Models.DTO.Objects;

public class EntityIdDto
{
    public Guid? Id { get; set; }

    public EntityIdDto(Guid? id)
    {
        Id = id;
    }
}