using ChartJs.Blazor.ChartJS.LineChart;
using dominikz.Client.Components;
using dominikz.Domain.Enums.Trades;
using dominikz.Domain.Filter;
using dominikz.Domain.ViewModels.Trading;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Pages.Trading;

public partial class Trades
{
    [Inject] protected TradesEndpoints? TradesEndpoints { get; set; }
    [Inject] protected DownloadEndpoints? DownloadEndpoints { get; set; }

    private LineConfig? _config;
    private List<EarningCallVm> _calls = new();
    private List<EarningCallVm> _released = new();
    private List<Timeline.TimelineEvent> _events = new();
    private EarningCallVm? _selected;
    
    protected override async Task OnInitializedAsync()
    {
        _calls = await TradesEndpoints!.SearchEarningsCalls(new EarningsCallsFilter());
        _released = _calls.Where(x => x.Growth != null)
            .OrderBy(x => x.Growth)
            .ThenBy(x => x.Surprise)
            .ToList();

        _events = _calls.Where(x => x.Growth == null)
            .Select(x => new Timeline.TimelineEvent()
            {
                Date = x.Date.ToDateTime(x.Release ?? new TimeOnly(13, 0, 0)),
                Description = x.Name,
                Title = x.Symbol,
                SymbolSrc = x.Sources.HasFlag(InformationSource.AktienFinder) ? x.LogoUrl : null
            }).OrderBy(x => x.Date)
            .ToList();
    }
    
    protected override void OnParametersSet()
    {
        // // Convert the CandleData to a format that Chart.js can use
        // var data = new
        // {
        //     labels = CandleDataList.Select(candle => candle.DateTime.ToString("yyyy-MM-dd")),
        //     datasets = new[]
        //     {
        //         new
        //         {
        //             label = "Close",
        //             data = CandleDataList.Select(candle => candle.Close),
        //             fill = false,
        //             borderColor = "rgb(75, 192, 192)",
        //             tension = 0.1
        //         }
        //     }
        // };
        //
        // // Convert the data to JSON and create the Chart.js configuration object
        // var dataJson = JsonConvert.SerializeObject(data);
        // _config = new LineConfig();
        // _config.Data.Labels.AddRange(data); = data.labels;
        // {
        //     Data = 
        //     Data = new ConfigData
        //     {
        //         Labels = data.labels,
        //         Datasets = data.datasets
        //     }
        // };
    }

    private void SelectEarningCall(EarningCallVm? selected)
        => _selected = selected;

    private void SelectEarningCall(Timeline.TimelineEvent tEvent)
        => _selected= _calls.FirstOrDefault(x => x.Symbol == tEvent.Title);
}