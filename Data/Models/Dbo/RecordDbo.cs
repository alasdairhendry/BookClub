using System.ComponentModel.DataAnnotations;

namespace Data.Models.Dbo;

public class RecordDbo
{
    public Guid Id { get; set; }

    [MaxLength(256)] public string Title { get; set; } = null!;
    [MaxLength(256)] public string Author { get; set; } = null!;
    [MaxLength(2048)] public string CoverUrl { get; set; } = null!;
    
    public string? ISBN { get; set; }
    public List<string> AdditionalISBNs { get; set; } = [];

    public DateTime? ReleaseDate { get; set; }
    public string? Publisher { get; set; }
}