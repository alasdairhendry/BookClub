using System.Net.Http.Json;
using Application.Interfaces;
using Application.Models.Dto.Actions;
using Application.Models.Dto.Objects;
using Application.Models.State;
using Domain.Models.OpenApi;

namespace Application.Services;

public class OpenApiService : IOpenApiService
{
    private HttpClient _apiClient;

    private string FormatQuery { get; init; }

    public OpenApiService()
    {
        _apiClient = new HttpClient();
        _apiClient.BaseAddress = new Uri("https://openlibrary.org");

        var acceptedFormats = new string[]
        {
            "paperback",
            "softcover",
            "hardcover",
            "mass market paperback",
            "ebook",
            "e-book",
            "audiobook",
            "eaudiobook",
            "e-audiobook",
            "audiobook on cassette",
            "audio cassette",
            "audio cd",
            "mp3 cd",
            "cd-rom",
        };

        FormatQuery = string.Join(" OR ", acceptedFormats.Select(x => $"\"{x}\""));
    }

    public async Task<ResultState<SearchResultDto>> SearchAsync(SearchRequestDto model)
    {
        try
        {
            var request = await _apiClient.GetAsync($"/search.json?q={model.SearchTerm} format:({FormatQuery})&limit={model.Limit}&offset={model.Offset}&lang=en&fields=key,title,subject,author_name,number_of_pages_median,format,editions,editions.key,editions.isbn,editions.cover_i");
            var response = await request.Content.ReadFromJsonAsync<SearchResult>();

            request.EnsureSuccessStatusCode();

            if (response == null)
                throw new Exception("Something went wrong");

            var result = new SearchResultDto()
            {
                SearchRequest = model,
                TotalResults = response.NumFound
            };

            foreach (var record in response.Docs)
            {
                var coverUrl = await ExtractImageUrl(record);

                result.Records.Add(new RecordDto
                {
                    Title = record.Title,
                    Author = record.AuthorName.FirstOrDefault(),
                    CoverUrl = coverUrl,
                    OpenLibraryId = record.Key,
                    Subjects = record.Subjects?.ToList(),
                    PageCountAverage = record.PageCountAverage,
                    ISBNs = record.ISBNs?.ToList()
                });
            }

            return ResultState<SearchResultDto>.Success(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<string?> ExtractImageUrl(SearchResultWork searchResultWork)
    {
        try
        {
            var mainEdition = searchResultWork.MainEdition?.Editions?.FirstOrDefault();

            if (mainEdition is null)
                return null;

            string? coverIdentifier = mainEdition.CoverId?.ToString();

            if (coverIdentifier is not null)
                return $"https://covers.openlibrary.org/b/id/{coverIdentifier}-L.jpg";

            coverIdentifier = mainEdition.ISBNs?.FirstOrDefault(x => x.StartsWith("978")) ?? mainEdition.ISBNs?.FirstOrDefault();

            if (coverIdentifier is not null)
                return $"https://covers.openlibrary.org/b/ISBN/{coverIdentifier}-L.jpg";

            coverIdentifier = mainEdition.Key?.Replace("/books/", "");

            if (coverIdentifier is not null)
                return $"https://covers.openlibrary.org/b/olid/{coverIdentifier}-L.jpg";

            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return string.Empty;
        }
    }
}