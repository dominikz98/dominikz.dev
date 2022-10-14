using dominikz.api.Extensions;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.kernel.Endpoints;
using dominikz.kernel.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Commands;

public class SearchRecipesQuery : RecipesFilter, IRequest<IReadOnlyCollection<RecipeVM>>
{ }

public class SearchRecipesQueryHandler : IRequestHandler<SearchRecipesQuery, IReadOnlyCollection<RecipeVM>>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public SearchRecipesQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<IReadOnlyCollection<RecipeVM>> Handle(SearchRecipesQuery request, CancellationToken cancellationToken)
    {
        var data = await _database.Recipes
            .Search(request)
            .Select(x => new
            {
                VM = new RecipeVM()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Duration = x.Duration,
                    Portions = x.Portions,
                    Categories = x.Categories.GetFlags(),
                    FoodCount = x.RecipesFoodsMappings.Count,
                    PricePerPortion = x.RecipesFoodsMappings.Sum(y => y.Multiplier * y.Food!.PricePerCount) / x.Portions
                },
                x.FileId
            })
            .OrderBy(x => x.VM.Title)
            .ToListAsync(cancellationToken);

        // attach image url
        foreach (var entry in data)
            entry.VM.ImageUrl = _linkCreator.Create(entry.FileId)?.ToString() ?? string.Empty;

        return data.Select(x => x.VM).ToList();
    }
}