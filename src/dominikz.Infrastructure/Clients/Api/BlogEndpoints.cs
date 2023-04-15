using dominikz.Domain.Enums.Blog;
using dominikz.Domain.Filter;
using dominikz.Domain.Options;
using dominikz.Domain.Structs;
using dominikz.Domain.ViewModels.Blog;
using dominikz.Infrastructure.Extensions;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure.Clients.Api;

public class BlogEndpoints
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
        => $"{_client.BaseUrl}{ApiClient.Prefix}/{Endpoint}/rss";

    public async Task<ArticleViewVm?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _client.GetSingle<ArticleViewVm>($"{Endpoint}/{id}", cancellationToken);
        AttachApiKey(vm);
        return vm;
    }

    public async Task<EditArticleVm?> GetDraftById(Guid id, CancellationToken cancellationToken = default)
        => await _client.GetSingle<EditArticleVm>($"{Endpoint}/draft/{id}", cancellationToken);

    public async Task<List<ArticleVm>> Search(ArticleFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<ArticleVm>($"{Endpoint}/search", filter, cancellationToken);
        foreach (var vm in vmList)
            AttachApiKey(vm);

        return vmList;
    }
    
    public async Task<int> SearchCount(ArticleFilter filter, CancellationToken cancellationToken = default)
        => await _client.GetSingle<int>($"{Endpoint}/search/count", filter, cancellationToken);

    public async Task<List<string>> GetTagsByCategory(ArticleCategoryEnum category, CancellationToken cancellationToken = default)
        => await _client.Get<string>($"{Endpoint}/tags/{category.ToString().ToLower()}", cancellationToken);

    public async Task<ArticleViewVm?> Add(EditArticleVm vm, List<FileStruct> files, CancellationToken cancellationToken = default)
        => await _client.Upload<EditArticleVm, ArticleViewVm>(HttpMethod.Post, Endpoint, vm, files, cancellationToken);

    public async Task<ArticleViewVm?> Update(EditArticleVm vm, List<FileStruct> files, CancellationToken cancellationToken = default)
        => await _client.Upload<EditArticleVm, ArticleViewVm>(HttpMethod.Put, Endpoint, vm, files, cancellationToken);

    private void AttachApiKey(ArticleVm? vm)
    {
        if (vm?.ImageUrl.StartsWith(_client.BaseUrl) ?? false)
            vm.AttachApiKey(_options.Value.Key);
    }

    public string CurlSearch(ArticleFilter filter)
        => _client.Curl($"{Endpoint}/search", filter);
}