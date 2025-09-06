using System.Text.Json.Serialization;

namespace Domain.Models.OpenApi;

public class SearchResultEditionDocument
{
    [JsonPropertyName("key")] 
    public string Key { get; set; } = null!;
    
    [JsonPropertyName("cover_i")] 
    public int? CoverId { get; set; }
    
    [JsonPropertyName("isbn")] 
    public string[]? ISBNs { get; set; } = [];
}