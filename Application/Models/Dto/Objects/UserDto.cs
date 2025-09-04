using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class UserDto
{
    public Guid Id { get; set; }
    public string EmailAddress { get; set; } = null!;
    public string Username { get; set; } = null!;
    
    public DateTime DateCreated { get; set; }
    public DateTime LastLogin { get; set; }
    
    public static UserDto FromDatabaseObject(ApplicationUserDbo model)
    {
        return new UserDto()
        {
            Id = model.Id,
            EmailAddress = model.Email!,
            Username = model.UserName!,
            DateCreated = model.DateCreated,
            LastLogin = model.LastLogin,
        };
    }
}