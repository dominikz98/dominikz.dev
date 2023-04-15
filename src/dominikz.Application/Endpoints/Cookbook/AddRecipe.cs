using dominikz.Application.Extensions;
using dominikz.Application.Utils;
using dominikz.Application.ViewModels;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Mapper;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Provider.Storage;
using dominikz.Infrastructure.Provider.Storage.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Application.Endpoints.Cookbook;

[Tags("cookbook")]
[Route("api/cookbook/recipes")]
[Authorize(Policy = Policies.Cookbook)]
[Authorize(Policy = Policies.CreateOrUpdate)]
public class AddRecipe : EndpointController
{
    private readonly IMediator _mediator;

    public AddRecipe(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Execute([FromForm] AddRecipeRequest request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        if (response.IsValid == false)
            return BadRequest(response.ToErrorList());

        return Ok(response.ViewModel);
    }
}

public class AddRecipeRequest : FileUploadWrapper<RecipeVm>, IRequest<ActionWrapper<RecipeDetailVm>>
{
}

public class EditRecipeRequestHandler : IRequestHandler<AddRecipeRequest, ActionWrapper<RecipeDetailVm>>
{
    private readonly IMediator _mediator;
    private readonly DatabaseContext _database;
    private readonly IStorageProvider _storage;

    public EditRecipeRequestHandler(IMediator mediator, DatabaseContext database, IStorageProvider storage)
    {
        _mediator = mediator;
        _database = database;
        _storage = storage;
    }

    public async Task<ActionWrapper<RecipeDetailVm>> Handle(AddRecipeRequest request, CancellationToken cancellationToken)
    {
        // verify
        var alreadyExists = await _database.From<Recipe>()
            .AnyAsync(x => EF.Functions.Like(x.Name, request.ViewModel.Name)
                           || x.Id == request.ViewModel.Id, cancellationToken);
        if (alreadyExists)
            return new("Recipe already exists");

        var file = request.Files.GetBySingleOrId(request.ViewModel.Id);
        if (file == null)
            return new("Invalid thumbnail");

        // create recipe
        var recipe = new Recipe().ApplyChanges(request.ViewModel);
        recipe.Created = DateTime.Now;

        // create and attach seps
        AttachSteps(recipe, request.ViewModel.Steps);

        // create, recalculate and attach ingredients
        await AttachIngredients(recipe, request.ViewModel.Ingredients, cancellationToken);

        // calculate nutri score
        RecipeHelper.CalculateNutriScore(recipe);

        await _database.AddAsync(recipe, cancellationToken);
        await _database.SaveChangesAsync(cancellationToken);

        // upload thumbnail
        var stream = file.OpenReadStream();
        stream.Position = 0;
        await _storage.Upload(new UploadImageRequest(recipe.Id, stream), cancellationToken);
        await _storage.Upload(new UploadImageRequest(request.ViewModel.Id, stream, ImageSizeEnum.ThumbnailHorizontal), cancellationToken);

        // map to viewmodel
        var vm = await _mediator.Send(new GetRecipeQuery(recipe.Id), cancellationToken);
        return vm == null
            ? new("Error loading recipe")
            : new ActionWrapper<RecipeDetailVm>(vm);
    }

    private void AttachSteps(Recipe recipe, List<RecipeStepVm> steps)
    {
        recipe.Steps = steps
            .Where(x => !string.IsNullOrWhiteSpace(x.Text))
            .OrderBy(x => x.Order)
            .Select(x => new RecipeStep().ApplyChanges(x, recipe.Id))
            .ToList();

        for (var i = 0; i < recipe.Steps.Count; i++)
            recipe.Steps[i].Order = i + 1;
    }

    private async Task AttachIngredients(Recipe recipe, List<IngredientVm> vms, CancellationToken cancellationToken)
    {
        var foodIds = vms
            .Select(x => x.Id)
            .Distinct()
            .ToList();

        var foods = await _database.From<Food>()
            .AsNoTracking()
            .Where(x => foodIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

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