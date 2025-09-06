using System.Text.Json.Serialization;

namespace Domain.Models.OpenApi;

public class SearchResultEdition
{
    [JsonPropertyName("docs")] 
    public SearchResultEditionDocument[]? Editions { get; set; }
    
}