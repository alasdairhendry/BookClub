using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;

namespace Application.Interfaces;

public interface IRecordService
{
    Task<ResultState<RecordDto?>> GetRecordAsync(Guid id);
    Task<ResultState<List<SuggestionResultDto>?>> GetSuggestionResultAsync(string request);
    Task<ResultState<SearchResultDto>> SearchAsync(SearchRequestDto model);


}