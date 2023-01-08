using dominikz.dev.Extensions;
using dominikz.dev.Models;
using dominikz.shared.Filter;
using dominikz.shared.ViewModels.Media;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Endpoints;

public class BookEndpoints
{
    private readonly ApiClient _client;
    private readonly IOptions<ApiOptions> _options;
    private const string Endpoint = "medias/books";

    public BookEndpoints(ApiClient client, IOptions<ApiOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<List<BookVm>> Search(BooksFilter filter, CancellationToken cancellationToken = default)
    {
        var vmList = await _client.Get<BookVm>($"{Endpoint}/search", filter, cancellationToken);
        vmList.AttachApiKey(_options.Value.Key);
        return vmList;
    }

    public string CurlSearch(BooksFilter filter)
        => _client.Curl($"{Endpoint}/search", filter);
}