using dominikz.Domain.Enums;
using dominikz.Domain.Extensions;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Media;

namespace dominikz.Infrastructure.Mapper;

public static class MovieMapper
{
    public static Movie ApplyChanges(this Movie original, EditMovieVm vm)
    {
        original.Id = vm.Id;
        original.YoutubeId = vm.YouTubeId;
        original.JustWatchId = vm.JustWatchId;
        original.ImdbId = vm.ImdbId;
        original.Title = vm.Title;
        original.PublishDate = vm.PublishDate!.Value;
        original.Category = MediaCategoryEnum.Movie;
        original.Comment = vm.Comment;
        original.Genres = (MovieGenresFlags)vm.Genres.Select(x => (int)x).Sum();
        original.Rating = vm.Rating;
        original.Year = vm.Year;
        original.Plot = vm.Plot;
        original.Runtime = vm.Runtime;
        return original;
    }

    public static IQueryable<EditMovieVm> MapToEditVm(this IQueryable<Movie> query)
        => query.Select(movie => new EditMovieVm()
        {
            Id = movie.Id,
            Title = movie.Title,
            PublishDate = movie.PublishDate,
            Year = movie.Year,
            Runtime = movie.Runtime,
            Plot = movie.Plot,
            Genres = movie.Genres.GetFlags(),
            Rating = movie.Rating,
            JustWatchId = movie.JustWatchId,
            ImdbId = movie.ImdbId,
            YouTubeId = movie.YoutubeId,
            Comment = movie.Comment,
            Stars = movie.MoviesPersonsMappings
                .Where(x => x.Category == PersonCategoryFlags.Star)
                .MapToEditVm()
                .ToList(),
            Writers = movie.MoviesPersonsMappings
                .Where(x => x.Category == PersonCategoryFlags.Writer)
                .MapToEditVm()
                .ToList(),
            Directors = movie.MoviesPersonsMappings
                .Where(x => x.Category == PersonCategoryFlags.Director)
                .MapToEditVm()
                .ToList()
        });
    
    public static IQueryable<MovieVm> MapToVm(this IQueryable<Movie> query)
        => query.Select(movie => new MovieVm()
        {
            Id = movie.Id,
            Title = movie.Title,
            PublishDate = movie.PublishDate,
            ImageUrl = movie.File!.Id.ToString(),
            Genres = movie.Genres,
            Rating = movie.Rating,
            Year = movie.Year
        });

    public static MovieDetailVm MapToDetailVm(this Movie movie)
        => new()
        {
            Id = movie.Id,
            Title = movie.Title,
            PublishDate = movie.PublishDate,
            ImageUrl = movie.File!.Id.ToString(),
            Genres = movie.Genres,
            Rating = movie.Rating,
            Comment = movie.Comment,
            Plot = movie.Plot,
            Runtime = movie.Runtime,
            YoutubeId = movie.YoutubeId,
            Year = movie.Year,
            Directors = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Director).MapToVm().ToList(),
            Writers = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Writer).MapToVm().ToList(),
            Stars = movie.MoviesPersonsMappings.Where(x => x.Category == PersonCategoryFlags.Star).MapToVm().ToList()
        };
}