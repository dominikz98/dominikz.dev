using System.Net.Http.Json;
using System.Text.Json;
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
    private readonly HttpClient _client;
    private readonly IOptions<ApiKeysOptions> _options;
    private const string LxExchange = "LSNG";

    public FinnhubClient(HttpClient client, IOptions<ApiKeysOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<IReadOnlyCollection<FhEarningSuprise>> GetEpsSurprises(string symbol, CancellationToken cancellationToken)
        => (await _client.GetFromJsonAsync<FhEarningSuprise[]>(
            $"api/v1/stock/earnings?symbol={symbol}&limit=1&token={_options.Value.Finnhub}",
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
            cancellationToken))!;

    public async Task<FhEarningList> GetEarningsCalendar(DateOnly timestamp, CancellationToken cancellationToken)
    {
        var to = timestamp.ToDateTime(new TimeOnly());
        var from = timestamp.AddDays(-1);
        return (await _client.GetFromJsonAsync<FhEarningList>(
            $"api/v1/calendar/earnings?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}&token={_options.Value.Finnhub}",
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
            cancellationToken))!;
    }

    public async Task<FhQuote?> GetQuoteBySymbol(string symbol, CancellationToken cancellationToken)
        => await _client.GetFromJsonAsync<FhQuote>(
            $"api/v1/stock/candle?symbol={symbol}&exchange={LxExchange}&token={_options.Value.Finnhub}",
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
            cancellationToken);

    public async Task<FhCandle?> GetCandles(string symbol, DateTime timestamp, CancellationToken cancellationToken)
    {
        var from = timestamp.AddMinutes(-15).ToUnixTimestamp();
        var to = timestamp.AddMinutes(15).ToUnixTimestamp();
        var resolution = 1;
        return await _client.GetFromJsonAsync<FhCandle>(
            $"api/v1/stock/candle?symbol={symbol}&resolution={resolution}&from={from}&to={to}&exchange={LxExchange}&token={_options.Value.Finnhub}",
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
            cancellationToken);
    }

    public async Task<FhCompany?> GetCompany(string symbol, CancellationToken cancellationToken)
        => await _client.GetFromJsonAsync<FhCompany>(
            $"api/v1/stock/profile2?symbol={symbol}&token={_options.Value.Finnhub}",
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
            cancellationToken);
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

public record FhCandle(
    int[] T,
    decimal[] O,
    decimal[] H,
    decimal[] L,
    decimal[] C,
    int[] V
);

public record FhQuote(
    decimal C,
    decimal H,
    decimal L,
    decimal O,
    decimal Pc,
    int T
);

public record FhEarningSuprise(
    decimal Actual,
    decimal Estimate,
    string Period,
    int Quarter,
    decimal Surprise,
    decimal SurprisePercent,
    string Symbol,
    int Year
);