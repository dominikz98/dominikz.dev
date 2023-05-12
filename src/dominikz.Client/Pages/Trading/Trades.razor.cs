using System.Globalization;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace dominikz.Client.Pages.Trading;

public partial class Trades
{
    [Inject] protected IJSRuntime? JsRuntime { get; set; }
    
    private BECanvasComponent? _canvasRef;
    private Canvas2DContext? _context;
    
    private const int ChartPadding = 20;
    private const int Width = 800;
    private const int Height = 400;
    private bool _isHovering;
    private double _hoverX;
    private const string ContainerId = "CanvasWrapper";
    
    private readonly List<(DateTime Timestamp, float Value)> _financialData = new()
    {
        (new DateTime(2023, 5, 1, 10, 0, 0), 100.0f),
        (new DateTime(2023, 5, 2, 9, 0, 0), 150.0f),
        (new DateTime(2023, 5, 3, 11, 0, 0), 120.0f),
        // Add more financial data points here
    };

    private readonly List<(DateTime Timestamp, string Name)> _events = new()
    {
        (new DateTime(2023, 5, 1, 12, 0, 0), "Event 1"),
        (new DateTime(2023, 5, 2, 13, 0, 0), "Event 2"),
        (new DateTime(2023, 5, 3, 14, 0, 0), "Event 3"),
        // Add more events here
    };

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _context = await _canvasRef.CreateCanvas2DAsync();
            await DrawChart();
            await JsRuntime!.InvokeVoidAsync("attachMouseMoveHandler", ContainerId, DotNetObjectReference.Create(this));
            await JsRuntime!.InvokeVoidAsync("attachMouseOutHandler", ContainerId, DotNetObjectReference.Create(this));
        }
    }

    private async Task DrawChart()
    {
        await _context!.ClearRectAsync(0, 0, 800, 400);
        
        var chartAreaWidth = Width - 2 * ChartPadding;
        var chartAreaHeight = Height - 2 * ChartPadding;

        // Calculate the minimum and maximum values
        var minValue = _financialData.Min(d => d.Value);
        var maxValue = _financialData.Max(d => d.Value);

        // Draw financial data dots and lines
        for (var i = 0; i < _financialData.Count; i++)
        {
            var x = MapTimestampToX(_financialData[i].Timestamp, chartAreaWidth) + ChartPadding;
            var y = MapValueToY(_financialData[i].Value, chartAreaHeight, minValue, maxValue) + ChartPadding;

            // Draw financial data dots
            await _context!.BeginPathAsync();
            await _context!.ArcAsync(x, y, 4, 0, 2 * Math.PI);
            await _context!.SetFillStyleAsync("#FFFFFF");
            await _context!.FillAsync();

            // Draw financial value below the dot
            await _context!.SetFillStyleAsync("#FFFFFF");
            await _context!.SetTextAlignAsync(TextAlign.Center);
            await _context!.SetFontAsync("12px Arial");
            await _context!.FillTextAsync(_financialData[i].Value.ToString(CultureInfo.InvariantCulture), x, y + 16);

            // Draw lines connecting financial data points
            if (i > 0)
            {
                var prevX = MapTimestampToX(_financialData[i - 1].Timestamp, chartAreaWidth) + ChartPadding;
                var prevY = MapValueToY(_financialData[i - 1].Value, chartAreaHeight, minValue, maxValue) + ChartPadding;
                await _context!.BeginPathAsync();
                await _context!.MoveToAsync(prevX, prevY);
                await _context!.LineToAsync(x, y);
                await _context!.SetStrokeStyleAsync("#FFFFFF");
                await _context!.StrokeAsync();
            }
        }

        // Draw event lines and names
        foreach (var eventData in _events)
        {
            var x = MapTimestampToX(eventData.Timestamp, chartAreaWidth) + ChartPadding;
            await _context!.BeginPathAsync();
            await _context!.MoveToAsync(x, ChartPadding);
            await _context!.LineToAsync(x, Height - ChartPadding);
            await _context!.SetStrokeStyleAsync("#FFA500");
            await _context!.StrokeAsync();

            // Draw event name below the line
            await _context!.SetFillStyleAsync("#FFFFFF");
            await _context!.SetTextAlignAsync(TextAlign.Center);
            await _context!.SetFontAsync("12px Arial");
            await _context!.FillTextAsync(eventData.Name, x, Height - 4);
        }

        // Draw the vertical stroke on hover
        if (_isHovering)
        {
            await _context!.BeginPathAsync();
            await _context!.MoveToAsync(_hoverX, ChartPadding);
            await _context!.LineToAsync(_hoverX, Height - ChartPadding);
            await _context!.SetStrokeStyleAsync("#FF0000");
            await _context!.StrokeAsync();
        }
    }

    private float MapTimestampToX(DateTime timestamp, float chartAreaWidth)
    {
        // Calculate the X position based on the timestamp and the available chart area width
        var timeSpan = timestamp - _financialData.Min(d => d.Timestamp);
        var totalMinutes = timeSpan.TotalMinutes;
        var minutesRange = _financialData.Max(d => (d.Timestamp - _financialData.Min(t => t.Timestamp)).TotalMinutes);
        return (float)(totalMinutes / minutesRange * chartAreaWidth);
    }

    private float MapValueToY(float value, float chartAreaHeight, float minValue, float maxValue)
    {
        // Calculate the Y position based on the value and the available chart area height
        var valueRange = maxValue - minValue;
        return chartAreaHeight - ((value - minValue) / valueRange * chartAreaHeight);
    }

    [JSInvokable]
    public async Task HandleMouseMove(int x)
    {
        _isHovering = true;
        _hoverX = x;
        await DrawChart();
    }

    [JSInvokable]
    public async Task HandleMouseOut()
    {
        _isHovering = false;
        await DrawChart();
    }
}

// using ChartJs.Blazor.ChartJS.LineChart;
// using dominikz.Client.Components;
// using dominikz.Domain.Enums.Trades;
// using dominikz.Domain.Filter;
// using dominikz.Domain.ViewModels.Trading;
// using dominikz.Infrastructure.Clients.Api;
// using Microsoft.AspNetCore.Components;
//
// namespace dominikz.Client.Pages.Trading;
//
// public partial class Trades
// {
//     [Inject] protected TradesEndpoints? TradesEndpoints { get; set; }
//     [Inject] protected DownloadEndpoints? DownloadEndpoints { get; set; }
//
//     private LineConfig? _config;
//     private List<EarningCallVm> _calls = new();
//     private List<EarningCallVm> _released = new();
//     private List<Timeline.TimelineEvent> _events = new();
//     private EarningCallVm? _selected;
//     
//     protected override async Task OnInitializedAsync()
//     {
//         _calls = await TradesEndpoints!.SearchEarningsCalls(new EarningsCallsFilter());
//         _released = _calls.Where(x => x.Growth != null)
//             .OrderBy(x => x.Growth)
//             .ThenBy(x => x.Surprise)
//             .ToList();
//
//         _events = _calls.Where(x => x.Growth == null)
//             .Select(x => new Timeline.TimelineEvent()
//             {
//                 Date = x.Date.ToDateTime(x.Release ?? new TimeOnly(13, 0, 0)),
//                 Description = x.Name,
//                 Title = x.Symbol,
//                 SymbolSrc = x.Sources.HasFlag(InformationSource.AktienFinder) ? x.LogoUrl : null
//             }).OrderBy(x => x.Date)
//             .ToList();
//     }
//     
//     protected override void OnParametersSet()
//     {
//         // // Convert the CandleData to a format that Chart.js can use
//         // var data = new
//         // {
//         //     labels = CandleDataList.Select(candle => candle.DateTime.ToString("yyyy-MM-dd")),
//         //     datasets = new[]
//         //     {
//         //         new
//         //         {
//         //             label = "Close",
//         //             data = CandleDataList.Select(candle => candle.Close),
//         //             fill = false,
//         //             borderColor = "rgb(75, 192, 192)",
//         //             tension = 0.1
//         //         }
//         //     }
//         // };
//         //
//         // // Convert the data to JSON and create the Chart.js configuration object
//         // var dataJson = JsonConvert.SerializeObject(data);
//         // _config = new LineConfig();
//         // _config.Data.Labels.AddRange(data); = data.labels;
//         // {
//         //     Data = 
//         //     Data = new ConfigData
//         //     {
//         //         Labels = data.labels,
//         //         Datasets = data.datasets
//         //     }
//         // };
//     }
//
//     private void SelectEarningCall(EarningCallVm? selected)
//         => _selected = selected;
//
//     private void SelectEarningCall(Timeline.TimelineEvent tEvent)
//         => _selected= _calls.FirstOrDefault(x => x.Symbol == tEvent.Title);
// }