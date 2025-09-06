using Application.Models.Dto.Actions;

namespace Application.Models.Dto.Objects;

public class SearchResultDto
{
    public List<RecordDto> Records { get; set; } = [];
    public int TotalResults { get; set; }
    
    public SearchRequestDto SearchRequest { get; set; } = null!;
}