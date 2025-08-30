using Microsoft.AspNetCore.Identity;

namespace Data.Models.Dbo;

public class ApplicationUserDbo : IdentityUser
{
    public List<ClubDbo> Clubs { get; set; }
}