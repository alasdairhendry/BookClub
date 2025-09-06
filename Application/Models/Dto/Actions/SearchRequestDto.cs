using System.ComponentModel;

namespace Application.Models.Dto.Actions;

public class SearchRequestDto
{
    [DefaultValue("The Final Empire")]
    public string SearchTerm { get; set; } = null!;
    public string? SearchField { get; set; } = null!;
    public string? SortBy { get; set; } = null!;
    [DefaultValue(20)]
    public int Limit { get; set; }
    public int Offset { get; set; }
}