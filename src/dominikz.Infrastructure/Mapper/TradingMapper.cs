using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Trading;

namespace dominikz.Infrastructure.Mapper;

public static class TradingMapper
{
    public static TradeDetailVm MapToVm(this Trade source)
        => new()
        {
            Id = source.Id,
            Name = source.Name,
            Date = source.Date,
            Timestamp = source.Timestamp,
            Symbol = source.Symbol,
            ISIN = source.ISIN,
            Fee = source.Fee,
            Tax = source.Tax,
            BuyIn = source.BuyIn,
            BuyOut = source.BuyOut
        };

    public static IReadOnlyCollection<EarningCallVm> MapToVm(this IReadOnlyCollection<EarningCall> source)
        => source.Select(x => new EarningCallVm()
        {
            Id = x.Id,
            Date = x.Date,
            Symbol = x.Symbol,
            Name = x.Name,
            Release = x.Release,
            Growth = x.Growth,
            Surprise = x.Surprise,
            ISIN = x.ISIN,
            Sources = x.Sources,
            AktienFinderLogoLink = x.AktienFinderLogoLink,
            OnVistaLink = x.OnVistaLink,
            OnVistaNewsLink = x.OnVistaNewsLink,
        }).ToList();
}