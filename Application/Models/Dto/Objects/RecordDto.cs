using Domain.Models.Dbo;

namespace Application.Models.Dto.Objects;

public class RecordDto
{
    public Guid Id { get; set; }
    public string? OpenLibraryId { get; set; }

    public string Title { get; set; } = null!;
    public string? Author { get; set; } = null!;
    
    public string? CoverUrl { get; set; } = null!;
    public string? PublicationYear { get; set; }
    public int? PageCountAverage { get; set; }

    public List<string>? Subjects { get; set; } = [];
    public List<string>? ISBNs { get; set; } = [];
    
    public static RecordDto FromDatabaseObject(RecordDbo model)
    {
        return new RecordDto()
        {
            Id = model.Id,
            OpenLibraryId = model.OpenLibraryId,
            Title = model.Title,
            Author = model.Author,
            CoverUrl = model.CoverUrl,
            PublicationYear = model.PublicationYear,
            PageCountAverage = model.PageCountAverage,
            ISBNs = model.ISBNs,
            Subjects = model.Subjects
        };
    }
    
    public static RecordDbo ToDatabaseObject(RecordDto model)
    {
        return new RecordDbo()
        {
            Id = model.Id,
            OpenLibraryId = model.OpenLibraryId,
            Title = model.Title,
            Author = model.Author,
            CoverUrl = model.CoverUrl,
            PublicationYear = model.PublicationYear,
            PageCountAverage = model.PageCountAverage,
            ISBNs = model.ISBNs ?? [],
            Subjects = model.Subjects ?? []
        };
    }
}