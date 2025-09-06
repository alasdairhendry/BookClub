using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;

namespace Application.Interfaces;

public interface IOpenApiService
{
    Task<ResultState<SearchResultDto>> SearchAsync(SearchRequestDto model);
}