using dominikz.Domain.Enums;
using dominikz.Domain.ViewModels.Trading;

namespace dominikz.Client.Api;

public class TradesEndpoints
{
    private readonly ApiClient _client;
    private const string Endpoint = "trades";

    public TradesEndpoints(ApiClient client)
    {
        _client = client;
    }

    public Task<EarningCallVm?> GetCallById(int id, CancellationToken cancellationToken = default)
        => Task.FromResult((EarningCallVm?)new EarningCallVm()
        {
            Id = 42,
            Company = "Test Inc. Corp.",
            IsReleased = true,
            Symbol = "TST",
            ISIN = "US1235212",
            LogoUrl = "https://img.rewe-static.de/8514957/32078973_digital-image.png",
            Timestamp = new DateTime(2023, 5, 13, 9, 30, 0),
            Time = EarningCallTime.BMO,
            EpsActual = 0.89m,
            EpsEstimate = 0.6m,
            RevenueActual = (long)49212.2,
            RevenueEstimate = (long)3999.4,
            LastStockPrice = 153.12m,
            Updated = new DateTime(2023, 5, 13, 23, 0, 2),
            Snapshots = 23,
            Trade = new TradeVm()
            {
                BuyIn = 151.24m,
                BuyInTimestamp = new DateTime(2023, 5, 13, 14, 0, 54),
                BuyOut = 164.63m,
                BuyOutTimestamp = new DateTime(2023, 5, 13, 17, 0, 30),
                StockCount = 8,
                Id = 492,
                Fee = 2,
                Tax = 8.24m
            },
            Externals = new ExternalUrl[]
            {
                new() { Name = "OnVista.de", Url = "https://www.onvista.de/" },
                new() { Name = "Finanzen.Net", Url = "https://www.finanzen.net/" },
                new() { Name = "MarketScreener.com", Url = "https://de.marketscreener.com/" }
            },
            Quarters = new EarningCallQuarter[]
            {
                new() { Name = "Q1", Value = 300.12m },
                new() { Name = "Q2", Value = 312.12m },
                new() { Name = "Q3", Value = 242.1m },
                new() { Name = "Q4", Value = 333.4m }
            },
            PriceSnapshots = new PriceSnapshot[]
            {
                new() { Timestamp = new DateTime(2023, 5, 13, 7, 30, 0), Value = 150.0m },
                new() { Timestamp = new DateTime(2023, 5, 13, 8, 0, 0), Value = 151.0m },
                new() { Timestamp = new DateTime(2023, 5, 13, 9, 0, 0), Value = 149.4m },
                new() { Timestamp = new DateTime(2023, 5, 13, 10, 0, 0), Value = 149.7m },
                new() { Timestamp = new DateTime(2023, 5, 13, 11, 0, 0), Value = 150.1m },
                new() { Timestamp = new DateTime(2023, 5, 13, 12, 0, 0), Value = 151.4m },
                new() { Timestamp = new DateTime(2023, 5, 13, 13, 0, 0), Value = 150.8m },
                new() { Timestamp = new DateTime(2023, 5, 13, 14, 0, 0), Value = 149.2m },
                new() { Timestamp = new DateTime(2023, 5, 13, 15, 0, 0), Value = 148.1m },
                new() { Timestamp = new DateTime(2023, 5, 13, 16, 0, 0), Value = 160.2m },
                new() { Timestamp = new DateTime(2023, 5, 13, 17, 0, 0), Value = 159.1m },
                new() { Timestamp = new DateTime(2023, 5, 13, 18, 0, 0), Value = 163.7m },
                new() { Timestamp = new DateTime(2023, 5, 13, 19, 0, 0), Value = 163.2m },
                new() { Timestamp = new DateTime(2023, 5, 13, 20, 0, 0), Value = 162.1m },
                new() { Timestamp = new DateTime(2023, 5, 13, 21, 0, 0), Value = 165.9m },
                new() { Timestamp = new DateTime(2023, 5, 13, 22, 0, 0), Value = 163.4m },
                new() { Timestamp = new DateTime(2023, 5, 13, 23, 0, 0), Value = 156.3m },
            },
            MarketEvents = new MarketEvent[]
            {
                new() { Timestamp = new DateTime(2023, 5, 13, 7, 30, 0), Name = "LS O" },
                new() { Timestamp = new DateTime(2023, 5, 13, 9, 30, 0), Name = "Release", },
                new() { Timestamp = new DateTime(2023, 5, 13, 15, 30, 0), Name = "NYSE O" },
                new() { Timestamp = new DateTime(2023, 5, 13, 21, 0, 0), Name = "NYSE C" },
                new() { Timestamp = new DateTime(2023, 5, 13, 23, 0, 0), Name = "LS C" },
            },
            BotEvents = new BotEvent[]
            {
                new() { Timestamp = new DateTime(2023, 5, 13, 6, 30, 5), Name = "Collect", Value = 150.0m },
                new() { Timestamp = new DateTime(2023, 5, 13, 14, 0, 54), Name = "Buy In", Value = 151.24m },
                new() { Timestamp = new DateTime(2023, 5, 13, 17, 0, 30), Name = "Buy Out", Value = 164.63m }
            }
        });

    public Task<IReadOnlyCollection<EarningCallListVm>> SearchCalls(CancellationToken cancellationToken = default)
        => Task.FromResult((IReadOnlyCollection<EarningCallListVm>)new List<EarningCallListVm>()
        {
            new()
            {
                Id = 42,
                Company = "Test Inc. Corp.",
                IsReleased = true,
                Symbol = "TST"
            }
        });
}