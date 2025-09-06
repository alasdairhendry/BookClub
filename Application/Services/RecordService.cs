using Application.Interfaces;
using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;
using Data.Repositories;
using Domain.Enums;
using Domain.Models.Dbo;

namespace Application.Services;

public class RecordService : IRecordService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOpenApiService _openApiService;

    public RecordService(IUnitOfWork unitOfWork, IOpenApiService openApiService)
    {
        _unitOfWork = unitOfWork;
        _openApiService = openApiService;
    }

    /// <summary>
    /// Gets a record from internal database
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ResultState<RecordDto?>> GetRecordAsync(Guid id)
    {
        try
        {
            var record = await _unitOfWork.GetRepository<RecordDbo>().GetAsync(id);

            if (record == null)
                return ResultState<RecordDto?>.Failed(ResultErrorType.NotFound, "Book not found");

            var dto = RecordDto.FromDatabaseObject(record);

            return ResultState<RecordDto?>.Success(dto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<ResultState<List<SuggestionResultDto>?>> GetSuggestionResultAsync(string request)
    {
        try
        {
            var result = await _unitOfWork.GetRepository<RecordDbo>().QueryAsync(
                x => x.Title.Contains(request, StringComparison.OrdinalIgnoreCase), limit: 5);

            var response = result.Select(x => new SuggestionResultDto
            {
                Id = x.Id,
                Title = x.Title,
                Author = x.Author
            }).ToList();

            return ResultState<List<SuggestionResultDto>?>.Success(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Performs a search against the IOpenApiService Search field
    /// Stores the returned records in DB, or fetches them if they already exist
    /// Returns the stored/fetched records
    /// </summary>
    public async Task<ResultState<SearchResultDto>> SearchAsync(SearchRequestDto model)
    {
        try
        {
            var search = await _openApiService.SearchAsync(model);

            var insertedOrFetchedRecords = await StoreAndReturnSearchResultsAsync(search.Data);
            // await PopulateSearchResultWithCachedRecordIds(search.Data, storedRecords);

            search.Data.Records = insertedOrFetchedRecords;

            return ResultState<SearchResultDto>.Success(search.Data);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    /// <summary>
    /// Takes in a search result from IOpenApiService (which doesn't have internal database IDs assigned)
    /// Stores them in the database, or fetches if a matching record already exists
    /// Returns the combined dataset of stored/fetched records to match the result from IOpenApiService
    /// </summary>
    private async Task<List<RecordDto>> StoreAndReturnSearchResultsAsync(SearchResultDto model)
    {
        try
        {
            var dbosFromModel = model.Records.Select(RecordDto.ToDatabaseObject).ToList();

            var openLibraryIds = dbosFromModel.Select(x => x.OpenLibraryId).ToList();

            var existing = (await _unitOfWork.GetRepository<RecordDbo>()
                .QueryAsync(x => openLibraryIds.Contains(x.OpenLibraryId))).ToList();

            for (var i = 0; i < dbosFromModel.Count; i++)
            {
                var existingDbo = existing.FirstOrDefault(x => x.OpenLibraryId == dbosFromModel[i].OpenLibraryId);

                if (existingDbo == null)
                {
                    await _unitOfWork.GetRepository<RecordDbo>().InsertAsync(dbosFromModel[i]);
                }
                else
                {
                    dbosFromModel[i] = existingDbo;
                }
            }

            // Don't add duplicate records
            // for (int i = dbosFromModel.Count - 1; i >= 0; i--)
            // {
            //     var existingDbo = existing.FirstOrDefault(x => x.OpenLibraryId == dbosFromModel[i].OpenLibraryId);
            //
            //     if (existingDbo != null)
            //     {
            //         dbosFromModel.RemoveAt(i);
            //     }
            // }

            // await _unitOfWork.GetRepository<RecordDbo>().InsertRangeAsync(dbosFromModel.ToList());
            await _unitOfWork.SaveAsync();

            return dbosFromModel.Select(RecordDto.FromDatabaseObject).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}