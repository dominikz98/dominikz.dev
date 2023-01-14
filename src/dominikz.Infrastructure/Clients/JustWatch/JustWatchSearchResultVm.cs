using System.Text.Json.Serialization;

namespace dominikz.Infrastructure.Clients.JustWatch;

internal class JustWatchSearchResultVm
{
    [JsonPropertyName("total_results")] public uint? TotalResults { get; set; }

    [JsonPropertyName("page")]
    public uint? Page { get; set; }

    [JsonPropertyName("page_size")]
    public uint? PageSize { get; set; }

    [JsonPropertyName("total_pages")]
    public uint? TotalPages { get; set; }

    [JsonPropertyName("items")]
    public List<JustWatchSearchResultItemVm> Items { get; set; } = new();
}

internal class JustWatchSearchResultItemVm
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;
}