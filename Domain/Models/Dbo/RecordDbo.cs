using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Dbo;

public class RecordDbo
{
    public Guid Id { get; set; }
    public string? OpenLibraryId { get; set; }

    [MaxLength(256)] public string Title { get; set; } = null!;
    [MaxLength(256)] public string? Author { get; set; } = null!;

    [MaxLength(2048)] public string? CoverUrl { get; set; } = null!;
    [MaxLength(4)] public string? PublicationYear { get; set; }
    public int? PageCountAverage { get; set; }

    // TODO - Pull Isbn10-13 Extraction from repo
    public List<string> ISBNs { get; set; } = [];
    public List<string> Subjects { get; set; } = [];
}