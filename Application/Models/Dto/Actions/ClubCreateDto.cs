using System.ComponentModel;

namespace Application.Models.Dto.Actions;

public class ClubCreateDto
{
    [DefaultValue("My Club!")] public string Name { get; set; } = null!;

    [DefaultValue("Hoo's got a match?")]
    public string? Motto { get; set; }
    
    [DefaultValue(true)]
    public bool IsPrivate { get; set; } = true;

    [DefaultValue("https://upload.wikimedia.org/wikipedia/en/a/a4/Who%27s_Got_a_Match_CD.jpg")]
    public string? ImageUrl { get; set; }
}