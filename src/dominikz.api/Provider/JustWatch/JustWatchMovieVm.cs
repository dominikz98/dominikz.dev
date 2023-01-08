using System.Text.Json.Serialization;

namespace dominikz.api.Provider.JustWatch;

public class JustWatchMovieVm
{
    public int Id { get; set; }
    public List<JustWatchOfferVm> Offers { get; set; } = new();
    public List<JustWatchClipVm> Clips { get; set; } = new();
}

public class JustWatchClipVm
{
    public string Type { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    [JsonPropertyName("external_id")] public string ExternalId { get; set; } = string.Empty;
}

public class JustWatchOfferVm
{
    [JsonPropertyName("monetization_type")] public string MonetizationType { get; set; } = string.Empty;
    [JsonPropertyName("provider_id")] public int ProviderId { get; set; }
    [JsonPropertyName("retail_price")] public decimal RetailPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    [JsonPropertyName("presentation_type")] public string PresentationType { get; set; } = string.Empty;
    [JsonPropertyName("date_provider_id")] public string DateProviderId { get; set; } = string.Empty;
}