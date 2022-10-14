using dominikz.api.Extensions;
using dominikz.api.Provider;
using dominikz.api.Utils;
using dominikz.kernel.ViewModels;
using Markdig;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Commands;

public class GetRecipeQuery : IRequest<RecipeDetailVM?>
{
    public Guid Id { get; set; }

    public GetRecipeQuery(Guid id)
    {
        Id = id;
    }
}

public class GetRecipeQueryHandler : IRequestHandler<GetRecipeQuery, RecipeDetailVM?>
{
    private readonly DatabaseContext _database;
    private readonly ILinkCreator _linkCreator;

    public GetRecipeQueryHandler(DatabaseContext database, ILinkCreator linkCreator)
    {
        _database = database;
        _linkCreator = linkCreator;
    }

    public async Task<RecipeDetailVM?> Handle(GetRecipeQuery request, CancellationToken cancellationToken)
    {
        var data = await _database.Recipes
            .Where(x => x.Id == request.Id)
            .Select(x => new
            {
                VM = new RecipeDetailVM()
                {
                    Id = x.Id,
                    Title = x.Title,
                    Duration = x.Duration,
                    Portions = x.Portions,
                    Categories = x.Categories.GetFlags(),
                    FoodCount = x.RecipesFoodsMappings.Count,
                    PricePerPortion = x.RecipesFoodsMappings.Sum(y => y.Multiplier * y.Food!.PricePerCount) / x.Portions,
                    Foods = x.RecipesFoodsMappings.Select(x => new FoodDetailVM()
                    {
                        Id = x.Food!.Id,
                        Title = x.Food!.Title,
                        PricePerCount = x.Food!.PricePerCount,
                        Icon = x.Food!.Icon,
                        Unit = x.Food!.Unit,
                        Count = x.Food!.Count,
                        Multiplier = x.Multiplier,
                        Kilocalories = x.Food.Kilocalories,
                        Protein = x.Food.Protein,
                        Fat = x.Food.Fat,
                        Carbohydrates = x.Food.Carbohydrates,
                        ReweUrl = x.Food.ReweUrl
                    })
                    .OrderBy(x => x.Title)
                    .ToList()
                },
                x.FileId,
                x.MDText
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (data is null)
            return null;

        // attach image url
        data.VM.ImageUrl = _linkCreator.Create(data.FileId)?.ToString() ?? string.Empty;

        // convert markdown to html5
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

        data.VM.HtmlText = Markdown.ToHtml(data.MDText, pipeline);
        return data.VM;
    }
}