using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;

namespace dominikz.Client.Pages.Trading;

public partial class Trades
{
    private BECanvasComponent? _canvasRef;
    private Canvas2DContext? _context;
    private const int ChartPadding = 20;
    private bool isHovering;
    private float mouseX;
    private string timestampDisplay;
    
    private List<(DateTime Timestamp, float Value)> financialData = new()
    {
        (new DateTime(2023, 5, 1, 10, 0, 0), 100.0f),
        (new DateTime(2023, 5, 2, 9, 0, 0), 150.0f),
        (new DateTime(2023, 5, 3, 11, 0, 0), 120.0f),
        // Add more financial data points here
    };

    private List<(DateTime Timestamp, string Name)> events = new List<(DateTime, string)>()
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
            await DrawChart(_context);
        }
    }

    private async Task DrawChart(Canvas2DContext context)
    {
        // Clear the canvas
        await context.ClearRectAsync(0, 0, 800, 400);

        float chartWidth = 800;
        float chartHeight = 400;
        float chartAreaWidth = chartWidth - 2 * ChartPadding;
        float chartAreaHeight = chartHeight - 2 * ChartPadding;

        // Calculate the minimum and maximum values
        float minValue = financialData.Min(d => d.Value);
        float maxValue = financialData.Max(d => d.Value);

        // Draw financial data dots and lines
        for (int i = 0; i < financialData.Count; i++)
        {
            float x = MapTimestampToX(financialData[i].Timestamp, chartAreaWidth) + ChartPadding;
            float y = MapValueToY(financialData[i].Value, chartAreaHeight, minValue, maxValue) + ChartPadding;

            // Draw financial data dots
            await context.BeginPathAsync();
            await context.ArcAsync(x, y, 4, 0, 2 * Math.PI);
            await context.SetFillStyleAsync("#FFFFFF");
            await context.FillAsync();

            // Draw financial value below the dot
            await context.SetFillStyleAsync("#FFFFFF");
            await context.SetTextAlignAsync(TextAlign.Center);
            await context.SetFontAsync("12px Arial");
            await context.FillTextAsync(financialData[i].Value.ToString(), x, y + 16);

            // Draw lines connecting financial data points
            if (i > 0)
            {
                float prevX = MapTimestampToX(financialData[i - 1].Timestamp, chartAreaWidth) + ChartPadding;
                float prevY = MapValueToY(financialData[i - 1].Value, chartAreaHeight, minValue, maxValue) + ChartPadding;
                await context.BeginPathAsync();
                await context.MoveToAsync(prevX, prevY);
                await context.LineToAsync(x, y);
                await context.SetStrokeStyleAsync("#FFFFFF");
                await context.StrokeAsync();
            }
        }

        // Draw event lines and names
        foreach (var eventData in events)
        {
            float x = MapTimestampToX(eventData.Timestamp, chartAreaWidth) + ChartPadding;
            await context.BeginPathAsync();
            await context.MoveToAsync(x, ChartPadding);
            await context.LineToAsync(x, chartHeight - ChartPadding);
            await context.SetStrokeStyleAsync("#FFA500");
            await context.StrokeAsync();

            // Draw event name below the line
            await context.SetFillStyleAsync("#FFFFFF");
            await context.SetTextAlignAsync(TextAlign.Center);
            await context.SetFontAsync("12px Arial");
            await context.FillTextAsync(eventData.Name, x, chartHeight - 4);
        }
    }

    private float MapTimestampToX(DateTime timestamp, float chartAreaWidth)
    {
        // Calculate the X position based on the timestamp and the available chart area width
        TimeSpan timeSpan = timestamp - financialData.Min(d => d.Timestamp);
        double totalMinutes = timeSpan.TotalMinutes;
        double minutesRange = financialData.Max(d => (d.Timestamp - financialData.Min(t => t.Timestamp)).TotalMinutes);
        return (float)(totalMinutes / minutesRange * chartAreaWidth);
    }

    private float MapValueToY(float value, float chartAreaHeight, float minValue, float maxValue)
    {
        // Calculate the Y position based on the value and the available chart area height
        float valueRange = maxValue - minValue;
        return chartAreaHeight - ((value - minValue) / valueRange * chartAreaHeight);
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