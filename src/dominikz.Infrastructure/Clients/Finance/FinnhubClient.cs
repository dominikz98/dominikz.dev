using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using dominikz.Domain.Options;
using dominikz.Infrastructure.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable NotAccessedPositionalProperty.Global

namespace dominikz.Infrastructure.Clients.Finance;

/// <summary>
/// More Information at https://finnhub.io/docs/api
/// </summary>
public class FinnhubClient
{
    public bool WaitWhenLimitReached { get; set; }

    private static int _retryCount;
    private readonly HttpClient _client;
    private readonly IOptions<ApiKeysOptions> _options;
    private const string LxExchange = "LSNG";

    public FinnhubClient(HttpClient client, IOptions<ApiKeysOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<FhSocialScore> GetSocialScores(string symbol, CancellationToken cancellationToken)
        => (await Get<FhSocialScore>($"api/v1/stock/social-sentiment?symbol={symbol}", cancellationToken))!;

    public async Task<IReadOnlyCollection<FhRecommendation>> GetRecommendations(string symbol, CancellationToken cancellationToken)
        => (await Get<FhRecommendation[]>($"api/v1/stock/recommendation?symbol={symbol}", cancellationToken))!;

    public async Task<IReadOnlyCollection<FhEarningSuprise>> GetEpsSurprises(string symbol, CancellationToken cancellationToken)
        => (await Get<FhEarningSuprise[]>($"api/v1/stock/earnings?symbol={symbol}&limit=1", cancellationToken))!;

    public async Task<FhEarningList> GetEarningsCalendar(DateTime from, DateTime to, CancellationToken cancellationToken)
        => (await Get<FhEarningList>($"api/v1/calendar/earnings?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}", cancellationToken))!;

    public async Task<FhQuote?> GetQuoteBySymbol(string symbol, CancellationToken cancellationToken)
        => await Get<FhQuote>($"api/v1/stock/candle?symbol={symbol}&exchange={LxExchange}", cancellationToken);

    public async Task<FhCandle> GetCandles(string symbol, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
    {
        var resolution = 1;
        return (await Get<FhCandle>($"api/v1/stock/candle?symbol={symbol}&resolution={resolution}&from={fromUtc.ToUnixTimestamp()}&to={toUtc.ToUnixTimestamp()}&exchange={LxExchange}", cancellationToken))!;
    }

    public async Task<FhCompany?> GetCompany(string symbol, CancellationToken cancellationToken)
        => await Get<FhCompany>($"api/v1/stock/profile2?symbol={symbol}", cancellationToken);

    private async Task<T?> Get<T>(string url, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _client.GetFromJsonAsync<T>(
                $"{url}&token={_options.Value.Finnhub}",
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
                cancellationToken);

            _retryCount = 0;
            return result;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode != HttpStatusCode.TooManyRequests)
                throw;

            if (WaitWhenLimitReached == false)
                throw;

            if (_retryCount > 0)
                throw;

            _retryCount++;
            await Task.Delay(TimeSpan.FromSeconds(65), cancellationToken);
            return await Get<T>(url, cancellationToken);
        }
    }
}

public record FhCompany(
    string Country,
    string Currency,
    string Exchange,
    string Ipo,
    decimal MarketCapitalization,
    string Name,
    string Phone,
    decimal ShareOutstanding,
    string Ticker,
    string Weburl,
    string Logo,
    string FinnhubIndustry
);

public record FhEarningList(
    FhEarning[] EarningsCalendar
);

public record FhEarning(
    DateTime Date,
    decimal? EpsActual,
    decimal? EpsEstimate,
    string Hour,
    int Quarter,
    long? RevenueActual,
    long? RevenueEstimate,
    string Symbol,
    int Year
);

public class FhCandle
{
    [JsonPropertyName("t")] public int[] Timestamp { get; set; } = Array.Empty<int>();
    [JsonPropertyName("o")] public decimal[] Open { get; set; } = Array.Empty<decimal>();
    [JsonPropertyName("h")] public decimal[] High { get; set; } = Array.Empty<decimal>();
    [JsonPropertyName("l")] public decimal[] Low { get; set; } = Array.Empty<decimal>();
    [JsonPropertyName("c")] public decimal[] Close { get; set; } = Array.Empty<decimal>();
    [JsonPropertyName("v")] public int[] Volume { get; set; } = Array.Empty<int>();
}

public record FhQuote(
    decimal C,
    decimal H,
    decimal L,
    decimal O,
    decimal Pc,
    int T
);

public record FhEarningSuprise(
    decimal? Actual,
    decimal? Estimate,
    DateTime Period,
    int Quarter,
    decimal? Surprise,
    decimal? SurprisePercent,
    string Symbol,
    int Year
);

public record FhRecommendation(
    DateTime Period,
    int Hold,
    int Buy,
    int StrongBuy,
    int Sell,
    int StrongSell,
    string Symbol
);

public record FhSocialScore(
    FhSocialMedia[] Reddit,
    FhSocialMedia[] Twitter,
    string Symbol
);

public record FhSocialMedia(
    DateTime AtTime,
    int Mention,
    decimal PositiveScore,
    decimal NegativeScore,
    int PositiveMention,
    int NegativeMention,
    decimal Score
);