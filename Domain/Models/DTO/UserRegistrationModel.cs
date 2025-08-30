namespace Domain.Models.DTO;

public class UserRegistrationModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}