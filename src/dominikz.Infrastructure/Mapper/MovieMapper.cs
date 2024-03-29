﻿using dominikz.Domain.Enums.Movies;
using dominikz.Domain.Extensions;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Movies;

namespace dominikz.Infrastructure.Mapper;

public static class MovieMapper
{
    public static Movie ApplyChanges(this Movie original, EditMovieVm vm)
    {
        original.Id = vm.Id;
        original.ImdbId = vm.ImdbId;
        original.Title = vm.Title;
        original.PublishDate = vm.PublishDate!.Value;
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
            ImdbId = movie.ImdbId,
            Comment = movie.Comment
        });

    public static IQueryable<MovieVm> MapToVm(this IQueryable<Movie> query)
        => query.Select(movie => new MovieVm()
        {
            Id = movie.Id,
            Title = movie.Title,
            PublishDate = movie.PublishDate,
            ImageUrl = movie.Id.ToString(),
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
            ImageUrl = movie.Id.ToString(),
            Genres = movie.Genres,
            Rating = movie.Rating,
            Comment = movie.Comment,
            Plot = movie.Plot,
            Runtime = movie.Runtime,
            Year = movie.Year,
            IsStreamAvailable = movie.FilePath != null,
            IsTrailerStreamAvailable = movie.TrailerPath != null
        };
    
    public static IQueryable<MoviePreviewVm> MapToPreviewVm(this IQueryable<Movie> query)
        => query.Select(media => new MoviePreviewVm()
        {
            Id = media.Id,
            Title = media.Title,
            PublishDate = media.PublishDate,
            ImageUrl = media.Id.ToString(),
        });
}