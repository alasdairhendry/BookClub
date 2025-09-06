namespace Domain.Models.OpenApi;

public class SearchResult
{
    public int NumFound { get; set; }
    public int Start { get; set; }
    public bool NumFoundExact { get; set; }
    public int Offset { get; set; }
    public SearchResultWork[] Docs { get; set; } = [];
}