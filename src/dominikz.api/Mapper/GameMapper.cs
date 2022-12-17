using dominikz.api.Models;
using dominikz.shared.ViewModels;

namespace dominikz.api.Mapper;

public static class GameMapper
{
    public static IQueryable<GameVM> MapToVm(this IQueryable<Game> query)
        => query.Select(game => new GameVM()
        {
            Id = game.Id,
            Title = game.Title,
            Timestamp = game.Timestamp,
            Image = game.File!.MapToVm(),
            Genres = game.Genres,
            Platform = game.Platform,
            Year = game.Year
        });
}
