using dominikz.Domain.Models;
using dominikz.Domain.ViewModels.Trading;

namespace dominikz.Infrastructure.Mapper;

public static class TradingMapper
{
    public static IQueryable<EarningCallVm> MapToVm(this IQueryable<EarningCall> source)
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
            OnVistaLink = x.OnVistaLink,
            OnVistaNewsLink = x.OnVistaNewsLink,
        });
}