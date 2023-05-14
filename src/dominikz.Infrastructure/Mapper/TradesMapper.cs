using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Trading;
using dominikz.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Infrastructure.Mapper;

public static class TradesMapper
{
    public static IQueryable<EarningCallListVm> MapToVm(this IQueryable<EarningCall> source)
        => source.Select(x => new EarningCallListVm()
        {
            Company = x.Company,
            Id = x.Id,
            Symbol = x.Symbol,
            IsReleased = x.UtcTimestamp <= DateTime.UtcNow.ToUnixTimestamp()
        });

    public static IQueryable<EarningCallVm> MapToDetailVm(this IQueryable<EarningCall> source)
        => source
            .Include(x => x.StockPrices)
            .Select(x => new EarningCallVm()
            {
                Company = x.Company,
                Id = x.Id,
                Symbol = x.Symbol,
                ISIN = x.ISIN,
                LogoUrl = x.LogoAvailable ? "X" : null,
                IsReleased = x.UtcTimestamp <= DateTime.UtcNow.ToUnixTimestamp(),
                Timestamp = x.UtcTimestamp.ToLocalDateTime(),
                Time = x.Time,
                EpsActual = x.EpsActual,
                EpsEstimate = x.EpsEstimate,
                RevenueActual = x.RevenueActual,
                RevenueEstimate = x.RevenueEstimate,
                Snapshots = x.StockPrices.Count,
                MarketEvents = new MarketEventVm[]
                {
                    new() { Timestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 30, 0, DateTimeKind.Utc).ToLocalTime(), Name = "LS O" },
                    new() { Timestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 30, 0, DateTimeKind.Utc).ToLocalTime(), Name = "NYSE O" },
                    new() { Timestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0, DateTimeKind.Utc).ToLocalTime(), Name = "NYSE C" },
                    new() { Timestamp = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 21, 0, 0, DateTimeKind.Utc).ToLocalTime(), Name = "LS C" },
                    new() { Timestamp = x.UtcTimestamp.ToLocalDateTime(), Name = "Release" },
                },
                LastStockPrice = x.StockPrices.OrderByDescending(y => y.UtcTimestamp).Select(y => y.Value).FirstOrDefault(),
                Updated = x.StockPrices.OrderByDescending(y => y.UtcTimestamp).Select(y => y.UtcTimestamp.ToLocalDateTime()).FirstOrDefault(),
                PriceSnapshots = x.StockPrices.Select(y => new PriceSnapshotVm()
                {
                    Timestamp = y.UtcTimestamp.ToLocalDateTime(),
                    Value = y.Value
                }).ToArray(),
                Quarters = new EarningCallQuarterVm[]
                {
                    new() { Name = "Q1", Value = x.Q1 },
                    new() { Name = "Q2", Value = x.Q2 },
                    new() { Name = "Q3", Value = x.Q3 },
                    new() { Name = "Q4", Value = x.Q4 }
                },
                Externals = new ExternalUrlVm[]
                {
                    new() { Name = "OnVista.de", Url = "https://www.onvista.de/" + x.ISIN },
                    new() { Name = "Finanzen.Net", Url = "https://www.finanzen.net/" + x.ISIN },
                    new() { Name = "MarketScreener.com", Url = "https://de.marketscreener.com/" + x.ISIN }
                }
            });
}