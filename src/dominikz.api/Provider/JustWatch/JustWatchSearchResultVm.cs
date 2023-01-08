using Newtonsoft.Json;

namespace dominikz.api.Provider.JustWatch;

internal class JustWatchSearchResultVm
{
    [JsonProperty("total_results")] public uint? TotalResults { get; set; }

    [JsonProperty("page", NullValueHandling = NullValueHandling.Ignore)]
    public uint? Page { get; set; }

    [JsonProperty("page_size", NullValueHandling = NullValueHandling.Ignore)]
    public uint? PageSize { get; set; }

    [JsonProperty("total_pages", NullValueHandling = NullValueHandling.Ignore)]
    public uint? TotalPages { get; set; }

    [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
    public List<JustWatchSearchResultItemVm> Items { get; set; } = new();
}

internal class JustWatchSearchResultItemVm
{
    [JsonProperty("id")] public long Id { get; set; }

    [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
    public string Title { get; set; } = string.Empty;
}