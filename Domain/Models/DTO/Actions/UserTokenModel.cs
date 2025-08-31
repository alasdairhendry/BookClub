namespace Domain.Models.DTO.Actions;

public class UserTokenModel
{
    public string Token { get; set; } = null!;
    public DateTime Expires { get; set; } = default!;
    
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshExpires { get; set; } = default!;
}