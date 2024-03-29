using System.ComponentModel.DataAnnotations;
using dominikz.Domain.Attributes;
using dominikz.Domain.Enums.Movies;

namespace dominikz.Domain.ViewModels.Movies;

public class EditMovieVm
{
    [GuidNotEmpty] public Guid Id { get; set; }
    [MinLength(5)] public string Title { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime? PublishDate { get; set; }

    [ListNotEmpty] public List<MovieGenresFlags> Genres { get; set; } = new();
    [Range(0, 100)] public int Rating { get; set; }
    [Range(1900, 2100)] public int Year { get; set; }
    [MinLength(5)] public string ImdbId { get; set; } = string.Empty;
    public string? Comment { get; set; }
    [MinLength(5)] public string Plot { get; set; } = string.Empty;
    [TimespanNotEmpty] public TimeSpan Runtime { get; set; }
}