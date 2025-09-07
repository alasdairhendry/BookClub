using System.ComponentModel;

namespace Application.Models.Dto.Actions;

public class ClubUpdateDto
{
    [DefaultValue("My Updated Club!")] public string? Name { get; set; } = null!;

    [DefaultValue("")]
    public string? Motto { get; set; }

    [DefaultValue(true)]
    public bool IsPrivate { get; set; } = true;
    
    [DefaultValue("")]
    public string? ImageUrl { get; set; }
}