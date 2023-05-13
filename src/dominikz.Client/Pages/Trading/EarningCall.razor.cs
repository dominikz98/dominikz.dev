using System.Globalization;
using dominikz.Client.Api;
using dominikz.Client.Components.Charts;
using dominikz.Domain.ViewModels.Trading;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Trading;

public partial class EarningCall
{
    [Parameter] public int Id { get; set; }
    [Inject] protected TradesEndpoints? Endpoints { get; set; }

    private CultureInfo _culture = CultureInfo.GetCultureInfo("de-DE");
    private EarningCallVm? _data;

    protected override async Task OnInitializedAsync()
    {
        _data = await Endpoints!.GetCallById(Id);
    }

    private List<LineChartValue> ConvertToLineChartEntry()
        => (_data?.MarketEvents
                .Select(x => new LineChartValue(x.Timestamp, x.Name, "#FFA500", true))
                .ToList() ?? new List<LineChartValue>())
            .Union(
                _data?.BotEvents
                    .Select(x => new LineChartValue(x.Timestamp, x.Name, "#5f7d8fFF", true))
                    .ToList() ?? new List<LineChartValue>()
            )
            .Union(
                _data?.BotEvents
                    .Select(x => new LineChartValue(x.Timestamp, $"{x.Value}", "#FFFFFF", false))
                    .ToList() ?? new List<LineChartValue>()
            )
            .Union(
                _data?.PriceSnapshots
                    .Select(x => new LineChartValue(x.Timestamp, $"{x.Value}", "#FFFFFF", false))
                    .ToList() ?? new List<LineChartValue>()
            )
            .OrderBy(x => x.Timestamp)
            .ToList();

    private List<BarChartItem> ConvertToBarChartEntry()
        => _data?.Quarters
            .Select(x => new BarChartItem(x.Name, x.Value))
            .ToList() ?? new List<BarChartItem>();

    private string? GetColorByValue(decimal? value)
        => GetColorByValue(value, 0);

    private string? GetColorByValue(decimal? first, decimal? second)
    {
        if ((first ?? 0) < (second ?? 0))
            return "ec-card-data-negative";

        if ((first ?? 0) > (second ?? 0))
            return "ec-card-data-positive";

        return null;
    }

    private TradeSummaryView CalculateTradeSummaryView()
    {
        if (_data?.Trade == null
            || _data.Trade.BuyOut == null)
            return new TradeSummaryView(0, 0, 0, 0, 0);

        var revenue = (_data.Trade.BuyOut.Value - _data.Trade.BuyIn) * _data.Trade.StockCount;
        var brutto = revenue - (_data.Trade.Fee ?? 0);
        var netto = brutto - (_data.Trade.Tax ?? 0);
        return new TradeSummaryView(revenue, brutto, netto, _data.Trade.Tax ?? 0, _data.Trade.Fee ?? 0);
    }

    private record TradeSummaryView(decimal Revenue, decimal Brutto, decimal Netto, decimal Tax, decimal Fee);
}