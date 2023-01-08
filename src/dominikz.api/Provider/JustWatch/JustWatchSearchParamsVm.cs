using Newtonsoft.Json;

namespace dominikz.api.Provider.JustWatch;

internal class JustWatchSearchParamsVm
{
    [JsonProperty("age_certifications")] public List<string> AgeCertifications { get; set; } = new();

    [JsonProperty("content_types")] public List<string> ContentTypes { get; set; } = new();

    [JsonProperty("presentation_types")] public List<string> PresentationTypes { get; set; } = new();

    [JsonProperty("providers")] public List<string> Providers { get; set; } = new();

    [JsonProperty("genres")] public List<string> Genres { get; set; } = new();

    [JsonProperty("languages")] public List<string> Languages { get; set; } = new();

    [JsonProperty("release_year_from")] public int? ReleaseYearFrom { get; set; }

    [JsonProperty("release_year_until")] public int? ReleaseYearUntil { get; set; }

    [JsonProperty("monetization_types")] public List<string> MonetizationTypes { get; set; } = new();

    [JsonProperty("min_price")] public string MinPrice { get; set; } = string.Empty;

    [JsonProperty("max_price")] public string MaxPrice { get; set; } = string.Empty;

    [JsonProperty("nationwide_cinema_releases_only")]
    public string NationwideCinemaReleasesOnly { get; set; } = string.Empty;

    [JsonProperty("scoring_filter_types")] public string ScoringFilterTypes { get; set; } = string.Empty;

    [JsonProperty("cinema_release")] public string CinemaRelease { get; set; } = string.Empty;

    [JsonProperty("query")] public string Query { get; set; } = string.Empty;

    [JsonProperty("page")] public int? Page { get; set; }

    [JsonProperty("page_size")] public int? PageSize { get; set; }

    [JsonProperty("timeline_type")] public string TimelineType { get; set; } = string.Empty;

    [JsonProperty("person_id")] public string PersonId { get; set; } = string.Empty;
}