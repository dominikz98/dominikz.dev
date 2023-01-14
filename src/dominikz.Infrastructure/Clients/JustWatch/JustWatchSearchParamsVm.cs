using System.Text.Json.Serialization;

namespace dominikz.Infrastructure.Clients.JustWatch;

internal class JustWatchSearchParamsVm
{
    [JsonPropertyName("age_certifications")] public List<string> AgeCertifications { get; set; } = new();

    [JsonPropertyName("content_types")] public List<string> ContentTypes { get; set; } = new();

    [JsonPropertyName("presentation_types")] public List<string> PresentationTypes { get; set; } = new();

    [JsonPropertyName("providers")] public List<string> Providers { get; set; } = new();

    [JsonPropertyName("genres")] public List<string> Genres { get; set; } = new();

    [JsonPropertyName("languages")] public List<string> Languages { get; set; } = new();

    [JsonPropertyName("release_year_from")] public int? ReleaseYearFrom { get; set; }

    [JsonPropertyName("release_year_until")] public int? ReleaseYearUntil { get; set; }

    [JsonPropertyName("monetization_types")] public List<string> MonetizationTypes { get; set; } = new();

    [JsonPropertyName("min_price")] public string MinPrice { get; set; } = string.Empty;

    [JsonPropertyName("max_price")] public string MaxPrice { get; set; } = string.Empty;

    [JsonPropertyName("nationwide_cinema_releases_only")]
    public string NationwideCinemaReleasesOnly { get; set; } = string.Empty;

    [JsonPropertyName("scoring_filter_types")] public string ScoringFilterTypes { get; set; } = string.Empty;

    [JsonPropertyName("cinema_release")] public string CinemaRelease { get; set; } = string.Empty;

    [JsonPropertyName("query")] public string Query { get; set; } = string.Empty;

    [JsonPropertyName("page")] public int? Page { get; set; }

    [JsonPropertyName("page_size")] public int? PageSize { get; set; }

    [JsonPropertyName("timeline_type")] public string TimelineType { get; set; } = string.Empty;

    [JsonPropertyName("person_id")] public string PersonId { get; set; } = string.Empty;
}