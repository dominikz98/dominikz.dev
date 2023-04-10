using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Media;

namespace dominikz.Infrastructure.Mapper;

public static class GameMapper
{
    public static IQueryable<GameVm> MapToVm(this IQueryable<Game> query)
        => query.Select(game => new GameVm()
        {
            Id = game.Id,
            Title = game.Title,
            PublishDate = game.PublishDate,
            ImageUrl = game.Id.ToString(),
            Genres = game.Genres,
            Platform = game.Platform,
            Year = game.Year
        });
}
