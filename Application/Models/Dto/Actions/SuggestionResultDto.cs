namespace Application.Models.Dto.Actions;

public class SuggestionResultDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Author { get; set; }
}