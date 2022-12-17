using dominikz.api.Models;
using dominikz.api.Utils;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class MovieMapper
{
    public static IQueryable<MovieVM> MapToVm(this IQueryable<Movie> query)
    => query.Select(movie => new MovieVM()
    {
        Id = movie.Id,
        Title = movie.Title,
        Timestamp = movie.Timestamp,
        Image = movie.File!.MapToVm(),
        Genres = movie.Genres,
        Rating = movie.Rating,
        Year = movie.Year
    });

    public static MovieDetailVM MapToDetailVm(this Movie movie)
        => new()
        {
            Id = movie.Id,
            Title = movie.Title,
            Timestamp = movie.Timestamp,
            Image = movie.File!.MapToVm(),
            Genres = movie.Genres,
            Rating = movie.Rating,
            Author = movie.Author?.MapToVm(),
            Comment = movie.MdText.ToHtml5(),
            Plot = movie.Plot.ToHtml5(),
            Runtime = movie.Runtime,
            YoutubeId = movie.YoutubeId,
            Year = movie.Year,
            Directors = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Director).MapToVm().ToList(),
            Writers = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Writer).MapToVm().ToList(),
            Stars = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Star).MapToVm().ToList()
        };
}
