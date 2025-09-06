using System.Text.Json.Serialization;

namespace Domain.Models.OpenApi;

public class SearchResultWork
{
    [JsonPropertyName("key")] 
    public string Key { get; set; }

    [JsonPropertyName("title")] 
    public string Title { get; set; }

    [JsonPropertyName("subject")] 
    public string[] Subjects { get; set; } = [];

    [JsonPropertyName("author_key")] 
    public string[] AuthorKeys { get; set; } = [];

    [JsonPropertyName("author_name")] 
    public string[] AuthorName { get; set; } = [];

    [JsonPropertyName("number_of_pages_median")]
    public int? PageCountAverage { get; set; }

    [JsonPropertyName("isbn")] 
    public string[] ISBNs { get; set; } = [];
    
    [JsonPropertyName("format")] 
    public string[] Formats { get; set; } = [];
    
    [JsonPropertyName("editions")] 
    public SearchResultEdition? MainEdition { get; set; }
    
}