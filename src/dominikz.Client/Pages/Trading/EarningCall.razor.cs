using System.Globalization;
using dominikz.Client.Api;
using dominikz.Client.Components.Charts;
using dominikz.Domain.ViewModels.Trading;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Trading;

public partial class EarningCall
{
    [Parameter] public int? Id { get; set; }
    [Parameter] public EarningCallVm? Data { get; set; }
    [Inject] protected TradesEndpoints? Endpoints { get; set; }

    private CultureInfo _culture = CultureInfo.GetCultureInfo("de-DE");
    private List<LineChartValue> _lineChartData = new();
    private List<BarChartValue> _barChartData = new();

    protected override async Task OnInitializedAsync()
    {
        if (Id == null)
            return;

        Data = await Endpoints!.GetCallById(Id.Value);
        if (Data == null)
            return;

        _lineChartData = ConvertToLineChartEntry();
        _barChartData = ConvertToBarChartEntry();
    }

    protected override void OnParametersSet()
    {
        if (Data == null)
            return;

        _lineChartData = ConvertToLineChartEntry();
        _barChartData = ConvertToBarChartEntry();
    }

    private List<LineChartValue> ConvertToLineChartEntry()
        => (Data?.MarketEvents
                .Select(x => new LineChartValue(x.Timestamp, x.Name, "#FFA500", true))
                .ToList() ?? new List<LineChartValue>())
            .Union(
                Data?.BotEvents
                    .Select(x => new LineChartValue(x.Timestamp, x.Name, "#5f7d8fFF", true))
                    .ToList() ?? new List<LineChartValue>()
            )
            .Union(
                Data?.BotEvents
                    .Select(x => new LineChartValue(x.Timestamp, $"{x.Value}", "#FFFFFF", false))
                    .ToList() ?? new List<LineChartValue>()
            )
            .Union(
                Data?.PriceSnapshots
                    .Select(x => new LineChartValue(x.Timestamp, $"{x.Value}", "#FFFFFF", false))
                    .ToList() ?? new List<LineChartValue>()
            )
            .OrderBy(x => x.Timestamp)
            .ToList();

    private List<BarChartValue> ConvertToBarChartEntry()
        => Data?.Quarters
            .Select(x => new BarChartValue(x.Name, x.Value))
            .ToList() ?? new List<BarChartValue>();

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
        if (Data?.Trade == null
            || Data.Trade.BuyOut == null)
            return new TradeSummaryView(0, 0, 0, 0, 0);

        var revenue = (Data.Trade.BuyOut.Value - Data.Trade.BuyIn) * Data.Trade.StockCount;
        var brutto = revenue - (Data.Trade.Fee ?? 0);
        var netto = brutto - (Data.Trade.Tax ?? 0);
        return new TradeSummaryView(revenue, brutto, netto, Data.Trade.Tax ?? 0, Data.Trade.Fee ?? 0);
    }

    private record TradeSummaryView(decimal Revenue, decimal Brutto, decimal Netto, decimal Tax, decimal Fee);
}