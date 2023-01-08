using dominikz.dev.Extensions;
using dominikz.dev.Models;
using dominikz.shared.Enums;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels.Blog;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Endpoints;

internal class BlogEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ApiOptions> _options;
    private const string Endpoint = "blog";

    public BlogEndpoints(ApiClient client, IOptions<ApiOptions> options)
    {
        _client = client;
        _options = options;
    }

    public string GetRssFeedUrl()
        => $"{_options.Value.Url}{ApiClient.Prefix}/{Endpoint}/rss";

    public async Task<ArticleViewVm?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _client.GetSingle<ArticleViewVm>($"{Endpoint}/{id}", cancellationToken);
        AttachApiKey(vm);
        return vm;
    }

    public async Task<EditArticleWrapper?> GetDraftById(Guid id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<EditArticleWrapper>($"{Endpoint}/draft/{id}", cancellationToken);

    public async Task<List<ArticleVm>> Search(ArticleFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<ArticleVm>($"{Endpoint}/search", filter, cancellationToken);
        foreach (var vm in vmList)
            AttachApiKey(vm);

        return vmList;
    }

    public async Task<List<string>> GetTagsByCategory(ArticleCategoryEnum category, CancellationToken cancellationToken = default)
        => await _client.Get<string>($"{Endpoint}/tags/{category.ToString().ToLower()}", cancellationToken);

    public async Task<ArticleViewVm?> Add(EditArticleWrapper vm, CancellationToken cancellationToken = default)
        => await _client.Upload<EditArticleVm, ArticleViewVm>(HttpMethod.Post, Endpoint, vm, vm.Images, cancellationToken);

    public async Task<ArticleViewVm?> Update(EditArticleWrapper vm, CancellationToken cancellationToken = default)
        => await _client.Upload<EditArticleVm, ArticleViewVm>(HttpMethod.Put, Endpoint, vm, vm.Images, cancellationToken);

    private void AttachApiKey(ArticleVm? vm)
    {
        if (vm?.ImageUrl.StartsWith(_options.Value.Url) ?? false)
            vm.AttachApiKey(_options.Value.Key);

        if (vm?.Author?.ImageUrl.StartsWith(_options.Value.Url) ?? false)
            vm.Author.AttachApiKey(_options.Value.Key);
    }

    public string CurlSearch(ArticleFilter filter)
        => _client.Curl($"{Endpoint}/search", filter);
}