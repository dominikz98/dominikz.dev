using dominikz.api.Models;
using dominikz.shared.ViewModels.Media;

namespace dominikz.api.Mapper;

public static class GameMapper
{
    public static IQueryable<GameVm> MapToVm(this IQueryable<Game> query)
        => query.Select(game => new GameVm()
        {
            Id = game.Id,
            Title = game.Title,
            PublishDate = game.PublishDate,
            ImageUrl = game.File!.Id.ToString(),
            Genres = game.Genres,
            Platform = game.Platform,
            Year = game.Year
        });
}
