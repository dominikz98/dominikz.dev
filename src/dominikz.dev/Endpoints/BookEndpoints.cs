using dominikz.shared.Filter;
using dominikz.shared.ViewModels;

namespace dominikz.dev.Endpoints;

public class BookEndpoints
{
    private readonly ApiClient _client;
    private static readonly string _endpoint = "medias/books";

    public BookEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<List<BookVM>> Search(BooksFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<BookVM>($"{_endpoint}/search", filter, cancellationToken);
}
