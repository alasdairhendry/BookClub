using System.ComponentModel;

namespace Application.Models.Dto.Actions;

public class UserRegistrationDto
{
    [DefaultValue("BobRossco")] public required string Username { get; set; }
    [DefaultValue("test@gmail.com")] public required string Email { get; set; }
    [DefaultValue("Passw0rd!")] public required string Password { get; set; }
    [DefaultValue("Passw0rd!")] public required string ConfirmPassword { get; set; }
}