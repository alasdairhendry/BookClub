using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Data.Models.Dbo;

public class ClubDbo
{
    public Guid Id { get; set; }
    
    [MaxLength(128)] public string Name { get; set; } = null!;
    [MaxLength(256)] public string? Motto { get; set; }
    [MaxLength(256)] public string? ImageUrl { get; set; }
    [MaxLength(256)] public bool IsPrivate { get; set; } = true;

    public List<ApplicationUserDbo> Members { get; set; } = [];
}