using System.ComponentModel;

namespace IntegrationTests.Models.DTO.Actions;

public class UserLoginDto
{
    [DefaultValue("test@gmail.com")]
    public required string Email { get; set; }
    [DefaultValue("Passw0rd!")]
    public required string Password { get; set; }
}