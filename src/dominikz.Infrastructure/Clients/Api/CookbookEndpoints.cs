using dominikz.Domain.Contracts;
using dominikz.Domain.Filter;
using dominikz.Domain.Options;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels.Cookbook;
using dominikz.Infrastructure.Extensions;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure.Clients.Api;

public class CookbookEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ApiOptions> _options;
    private const string Endpoint = "cookbook";

    public CookbookEndpoints(ApiClient client, IOptions<ApiOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<IReadOnlyCollection<FoodListVm>> GetFoods(CancellationToken cancellationToken = default)
        => await _client.Get<FoodListVm>($"{Endpoint}/foods", cancellationToken);

    public async Task<FoodVm?> GetFoodDraftById(Guid id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<FoodVm>($"{Endpoint}/foods/{id}", cancellationToken);

    public async Task<FoodVm?> GetFoodTemplateById(int id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<FoodVm>($"{Endpoint}/foods/template/{id}", cancellationToken);

    public async Task<FoodVm?> AddFood(FoodVm vm, CancellationToken cancellationToken = default)
        => await _client.Post<FoodVm, FoodVm>($"{Endpoint}/foods", vm, false, cancellationToken);

    public async Task<FoodVm?> UpdateFood(FoodVm vm, CancellationToken cancellationToken = default)
        => await _client.Put<FoodVm, FoodVm>($"{Endpoint}/foods", vm, false, cancellationToken);


    public async Task<List<RecipeListVm>> Search(RecipeFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<RecipeListVm>($"{Endpoint}/recipes/search", filter, cancellationToken);
        foreach (var vm in vmList)
            AttachApiKey(vm);

        return vmList;
    }

    public async Task<RecipeVm?> GetDraftById(Guid id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<RecipeVm>($"{Endpoint}/recipes/draft/{id}", cancellationToken);

    public async Task<RecipeDetailVm?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _client.GetSingle<RecipeDetailVm>($"{Endpoint}/recipes/{id}", cancellationToken);
        AttachApiKey(vm);
        return vm;
    }

    public async Task<RecipeVm?> Add(RecipeVm vm, List<FileStruct> files, CancellationToken cancellationToken = default)
        => await _client.Upload<RecipeVm, RecipeVm>(HttpMethod.Post, $"{Endpoint}/recipes", vm, files, cancellationToken);

    public async Task<RecipeVm?> Update(RecipeVm vm, List<FileStruct> files, CancellationToken cancellationToken = default)
        => await _client.Upload<RecipeVm, RecipeVm>(HttpMethod.Put, $"{Endpoint}/recipes", vm, files, cancellationToken);

    public string CurlSearch(RecipeFilter filter)
        => _client.Curl($"{Endpoint}/recipes/search", filter);

    private void AttachApiKey(IHasImageUrl? vm)
    {
        if (vm?.ImageUrl.StartsWith(_client.BaseUrl) ?? false)
            vm.AttachApiKey(_options.Value.ApiKey);
    }
}