using System.ComponentModel;

namespace Domain.Models.DTO;

public class ClubUpdateDto
{
    public Guid Id { get; set; }
    
    [DefaultValue("My Updated Club!")] public string? Name { get; set; } = null!;

    [DefaultValue("")]
    public string? Motto { get; set; }

    [DefaultValue(true)]
    public bool IsPrivate { get; set; } = true;
    
    [DefaultValue("")]
    public string? ImageUrl { get; set; }
}