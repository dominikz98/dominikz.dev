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

    public async Task<FhQuote?> GetQuoteBySymbol(string symbol, CancellationToken cancellationToken)
        => await _client.GetFromJsonAsync<FhQuote>(
            $"api/v1/stock/candle?symbol={symbol}&exchange={LxExchange}&token={_options.Value.Finnhub}",
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
            cancellationToken);

    public async Task<FhCandle?> GetCandlesByISIN(string symbol, DateTime timestamp, CancellationToken cancellationToken)
    {
        var from = timestamp.AddMinutes(-15).ToUnixTimestamp();
        var to = timestamp.AddMinutes(15).ToUnixTimestamp();
        var resolution = 1;
        return await _client.GetFromJsonAsync<FhCandle>(
            $"api/v1/stock/candle?symbol={symbol}&resolution={resolution}&from={from}&to={to}&exchange={LxExchange}&token={_options.Value.Finnhub}",
            new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
            cancellationToken);
    }

    public async Task<FhCompany?> TryGetCompanyBySymbol(string symbol, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.GetFromJsonAsync<FhCompany>(
                $"api/v1/stock/profile2?symbol={symbol}&token={_options.Value.Finnhub}",
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    public async Task<FhCompany?> TryGetCompanyByISIN(string isin, CancellationToken cancellationToken)
    {
        try
        {
            return await _client.GetFromJsonAsync<FhCompany>(
                $"api/v1/stock/profile2?isin={isin}&token={_options.Value.Finnhub}",
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true },
                cancellationToken);
        }
        catch (HttpRequestException)
        {
            return null;
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
    double ShareOutstanding,
    string Ticker,
    string Weburl,
    string Logo,
    string FinnhubIndustry
);

public record FhCandle(
    int[] T,
    double[] O,
    double[] H,
    double[] L,
    double[] C,
    int[] V
);

public record FhQuote(
    double C,
    double H,
    double L,
    double O,
    double Pc,
    int T
);