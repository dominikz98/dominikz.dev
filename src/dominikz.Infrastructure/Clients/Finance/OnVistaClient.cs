using System.Net.Http.Json;

// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace dominikz.Infrastructure.Clients.Finance;

public class OnVistaClient
{
    public async Task<OvResult?> GetStockByISIN(string isin, CancellationToken cancellationToken)
    {
        var url = $"https://api.onvista.de/api/v1/instruments/search/facet?perType=10&searchValue={isin}";
        var result = await new HttpClient().GetFromJsonAsync<OvQueryResult>(url, cancellationToken);

        return result?.Facets?.Where(x => x.Type.Equals("Stock", StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.Results ?? new List<OvResult>())
            .FirstOrDefault(x => x.ISIN == isin);
    }
    
    public async Task<OvResult?> GetStockBySymbolAndName(string symbol, string name, CancellationToken cancellationToken)
    {
        // cleanup name
        var cleanedName = name.Replace(".", "")
            .Replace(",", "")
            .Replace("Corp", "")
            .Replace("Inc", "");
   
        var url = $"https://api.onvista.de/api/v1/instruments/search/facet?perType=10&searchValue={cleanedName}";
        var result = await new HttpClient().GetFromJsonAsync<OvQueryResult>(url, cancellationToken);

        return result?.Facets?.Where(x => x.Type.Equals("Stock", StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.Results ?? new List<OvResult>())
            .FirstOrDefault(x => x.Symbol == symbol);
    }

    private async Task<OvResult?> GetAndValidateBySymbol(string symbol, string keyword, CancellationToken cancellationToken)
    {
        var url = $"https://api.onvista.de/api/v1/instruments/search/facet?perType=10&searchValue={keyword}";
        var result = await new HttpClient().GetFromJsonAsync<OvQueryResult>(url, cancellationToken);

        return result?.Facets?.Where(x => x.Type.Equals("Stock", StringComparison.OrdinalIgnoreCase))
            .SelectMany(x => x.Results ?? new List<OvResult>())
            .FirstOrDefault(x => x.Symbol == symbol);
    }
}

public class OvAdditionalData
{
    public string CountryCode { get; set; } = string.Empty;
    public int? IdIssuer { get; set; }
    public string NameIssuer { get; set; } = string.Empty;
    public int? IdGroupIssuer { get; set; }
    public string NameGroupIssuer { get; set; } = string.Empty;
    public string NameInstrumentSubType { get; set; } = string.Empty;
    public string NameInvestmentFocus { get; set; } = string.Empty;
    public DateTime? DateMaturity { get; set; }
    public string IndexFamily { get; set; } = string.Empty;
}

internal class OvFacet
{
    public string Type { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public int TotalAmount { get; set; }
    public string FinderPath { get; set; } = string.Empty;
    public List<OvResult>? Results { get; set; }
}

public class OvResult
{
    public string Type { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public List<string>? EntityAttributes { get; set; }
    public string EntityValue { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public OvUrls Urls { get; set; } = new();
    public string InstrumentType { get; set; } = string.Empty;
    public string ISIN { get; set; } = string.Empty;
    public string Wkn { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string DisplayType { get; set; } = string.Empty;
    public string UrlName { get; set; } = string.Empty;
    public OvAdditionalData? AdditionalData { get; set; }
    public string EntitySubType { get; set; } = string.Empty;
}

internal class OvQueryResult
{
    public long Expires { get; set; }
    public string SearchValue { get; set; } = string.Empty;
    public List<OvFacet>? Facets { get; set; }
}

public class OvUrls
{
    public string News { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}