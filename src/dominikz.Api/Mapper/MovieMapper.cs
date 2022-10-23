using dominikz.api.Models;
using dominikz.api.Utils;
using dominikz.kernel.Contracts;
using dominikz.kernel.ViewModels;

namespace dominikz.api.Mapper;

public static class MovieMapper
{
    public static IQueryable<MovieVM> MapToVM(this IQueryable<Movie> query)
    => query.Select(movie => new MovieVM()
    {
        Id = movie.Id,
        Title = movie.Title,
        Timestamp = movie.Timestamp,
        Image = movie.File!.MapToVM(),
        Genres = movie.Genres,
        Rating = movie.Rating,
        Year = movie.Year
    });

    public static MovieDetailVM MapToDetailVM(this Movie movie)
        => new()
        {
            Id = movie.Id,
            Title = movie.Title,
            Timestamp = movie.Timestamp,
            Image = movie.File!.MapToVM(),
            Genres = movie.Genres,
            Rating = movie.Rating,
            Author = movie.Author?.MapToVM(),
            Comment = movie.MDText.ToHtml5(),
            Plot = movie.Plot.ToHtml5(),
            Runtime = movie.Runtime,
            YoutubeId = movie.YoutubeId,
            Year = movie.Year,
            Directors = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Director).MapToVM().ToList(),
            Writers = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Writer).MapToVM().ToList(),
            Stars = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Star).MapToVM().ToList()
        };
}
