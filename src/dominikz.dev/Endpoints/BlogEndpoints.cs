using dominikz.dev.Models;
using dominikz.dev.Utils;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Endpoints;

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
        => $"{_options.Value.Url}{ApiClient.Prefix}/{Endpoint}/rss";

    public async Task<ArticleDetailVm?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var vm = await _client.GetSingle<ArticleDetailVm>($"{Endpoint}/{id}", cancellationToken);
        AttachApiKey(vm);
        return vm;
    }

    public async Task<List<ArticleListVm>> Search(ArticleFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<ArticleListVm>($"{Endpoint}/search", filter, cancellationToken);
        foreach (var vm in vmList)
            AttachApiKey(vm);

        return vmList;
    }

    private void AttachApiKey(ArticleVm? vm)
    {
        if (string.IsNullOrWhiteSpace(vm?.Image?.Url) == false
            && vm.Image.Url.StartsWith(_options.Value.Url))
            vm.Image.Url = QueryUtils.AttachParam(vm.Image.Url, ApiClient.ApiKeyHeaderName, _options.Value.Key);

        if (string.IsNullOrWhiteSpace(vm?.Author?.Image?.Url) == false
            && vm.Author.Image.Url.StartsWith(_options.Value.Url))
            vm.Author.Image.Url = QueryUtils.AttachParam(vm.Author.Image.Url, ApiClient.ApiKeyHeaderName, _options.Value.Key);
    }

    public string CurlSearch(ArticleFilter filter)
        => _client.Curl($"{Endpoint}/search", filter);
}