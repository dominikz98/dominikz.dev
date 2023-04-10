using dominikz.Application.Extensions;
using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Provider.Storage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Cookbook;

[Tags("cookbook")]
[Route("api/cookbook/recipes")]
[Authorize(Policy = Policies.Cookbook)]
[Authorize(Policy = Policies.CreateOrUpdate)]
public class UpdateRecipe : EndpointController
{
    private readonly IMediator _mediator;

    public UpdateRecipe(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPut]
    public async Task<IActionResult> Execute([FromForm] UpdateRecipeRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public class UpdateRecipeRequest : FileUploadWrapper<RecipeVm>, IRequest<ActionWrapper<RecipeDetailVm>>
{
}

public class UpdateRecipeRequestHandler : IRequestHandler<UpdateRecipeRequest, ActionWrapper<RecipeDetailVm>>
{
    private readonly IMediator _mediator;
    private readonly DatabaseContext _database;
    private readonly IStorageProvider _storage;

    public UpdateRecipeRequestHandler(IMediator mediator, DatabaseContext database, IStorageProvider storage)
    {
        _mediator = mediator;
        _database = database;
        _storage = storage;
    }

    public async Task<ActionWrapper<RecipeDetailVm>> Handle(UpdateRecipeRequest request, CancellationToken cancellationToken)
    {
        // verify
        var recipe = await _database.From<Recipe>()
            .Include(x => x.Ingredients)
            .Include(x => x.Steps)
            .FirstOrDefaultAsync(x => x.Id == request.ViewModel.Id, cancellationToken);

        if (recipe == null)
            return new("Recipe not found");

        var file = request.Files.GetBySingleOrId(recipe.Id);
        if (file == null)
            return new("Invalid thumbnail");

        // apply changes
        recipe = recipe.ApplyChanges(request.ViewModel);

        // update seps
        UpdateSteps(recipe, request.ViewModel.Steps);

        // create, recalculate and attach ingredients
        await UpdateIngredients(recipe, request.ViewModel.Ingredients, cancellationToken);

        // calculate nutri score
        RecipeHelper.CalculateNutriScore(recipe);
        _database.Update(recipe);
        await _database.SaveChangesAsync(cancellationToken);

        // upload thumbnail
        var stream = file.OpenReadStream();
        stream.Position = 0;
        await _storage.Upload(new UploadImageRequest(recipe.Id, stream), cancellationToken);

        // map to viewmodel
        var vm = await _mediator.Send(new GetRecipeQuery(recipe.Id), cancellationToken);
        return vm == null
            ? new("Error loading recipe")
            : new ActionWrapper<RecipeDetailVm>(vm);
    }

    private void UpdateSteps(Recipe recipe, List<RecipeStepVm> steps)
    {
        recipe.Steps = steps
            .Where(x => !string.IsNullOrWhiteSpace(x.Text))
            .OrderBy(x => x.Order)
            .Select(x => new RecipeStep().ApplyChanges(x, recipe.Id))
            .ToList();

        for (var i = 0; i < recipe.Steps.Count; i++)
            recipe.Steps[i].Order = i + 1;
    }

    private async Task UpdateIngredients(Recipe recipe, List<IngredientVm> vms, CancellationToken cancellationToken)
    {
        var foodIds = vms
            .Select(x => x.Id)
            .Distinct()
            .ToList();

        var foods = await _database.From<Food>()
            .Where(x => foodIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        recipe.Ingredients = new List<Ingredient>();
        foreach (var vm in vms)
        {
            var food = foods.FirstOrDefault(x => x.Id == vm.Id);
            if (food == null)
                continue;

            var ingredient = new Ingredient().ApplyChanges(vm, recipe.Id);
            ingredient.Factor = RecipeHelper.CalculateFactorByUnit(vm, food);
            ingredient.Food = food;
            recipe.Ingredients.Add(ingredient);
        }
        
    }
}