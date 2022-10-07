using dominikz.kernel.ViewModels;

namespace dominikz.kernel.Endpoints;

public class MediasEndpoints
{
    private readonly ApiClient _client;
    private static readonly string _endpoint = "medias";

    public MediasEndpoints(ApiClient client)
    {
        _client = client;
    }

    public async Task<MediaVM?> GetById(Guid id, CancellationToken cancellationToken = default)
        => await _client.Get<MediaVM>(_endpoint, id, cancellationToken);

    public async Task<List<MediaVM>> Search(MediaFilter filter, CancellationToken cancellationToken = default)
        => await _client.Get<MediaVM>($"{_endpoint}/search", filter, cancellationToken);
}

public class MediaFilter : IFilter
{
    public Guid? Id { get; set; }
    public MediaCategoryEnum Category { get; set; }
    public MediaGenre Genre { get; set; }
    public string? Text { get; set; }
    public int? Index { get; set; }
    public int? Count { get; set; }

    public static MediaFilter Default => new();
    public static MediaFilter Preview => new() { Count = 5 };

    public Dictionary<string, string> GetParameter()
    {
        var result = new Dictionary<string, string>();

        if (Id is not null)
            result.Add(nameof(Id), Id.ToString()!);

        if (Text is not null)
            result.Add(nameof(Text), Text);

        if (Category != MediaCategoryEnum.ALL)
            result.Add(nameof(Category), Category.ToString()!);

        if (Genre != MediaGenre.ALL)
            result.Add(nameof(Genre), Genre.ToString()!);

        if (Index is not null)
            result.Add(nameof(Index), Index.ToString()!);

        if (Count is not null)
            result.Add(nameof(Count), Count.ToString()!);

        return result;
    }
}