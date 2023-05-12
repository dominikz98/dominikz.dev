using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace dominikz.Client.Components.Charts;

public partial class LineChart
{
    [Inject] protected IJSRuntime? JsRuntime { get; set; }
    [Parameter] public List<LineChartValue> Values { get; set; } = new();

    private BECanvasComponent? _canvasRef;
    private Canvas2DContext? _context;

    private const int ChartPadding = 20;
    private const int Width = 800;
    private const int Height = 300;
    private bool _isHovering;
    private double _hoverX;
    private const string ContainerId = "CanvasWrapper";

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
        if (Values.Count == 0)
            return;

        var orderedValues = Values.OrderBy(x => x.Timestamp).ToList();

        await _context!.ClearRectAsync(0, 0, 800, 400);

        var chartAreaWidth = Width - 2 * ChartPadding;
        var chartAreaHeight = Height - 2 * ChartPadding;

        var isFirstPoint = true;
        var previousX = 0f;
        var previousY = 0f;
        var previousValue = 0m;
        var lineColor = "#FFFFFF"; // Default color is white

        // Calculate the minimum and maximum values
        var minValue = (float)orderedValues.Where(x => double.TryParse(x.Value, out _)).Min(d => Convert.ToDouble(d.Value));
        var maxValue = (float)orderedValues.Where(x => double.TryParse(x.Value, out _)).Max(d => Convert.ToDouble(d.Value));

        // Draw chart data dots and lines
        foreach (var entry in orderedValues)
        {
            var x = MapTimestampToX(entry.Timestamp, chartAreaWidth) + ChartPadding;
            var y = MapValueToY(entry.Value, chartAreaHeight, minValue, maxValue) + ChartPadding;

            if (entry.IsEvent)
            {
                // Handle event markers
                await _context!.SetFillStyleAsync("#FFA500");
                await _context!.BeginPathAsync();
                await _context!.MoveToAsync(x, ChartPadding);
                await _context!.LineToAsync(x, Height - ChartPadding);
                await _context!.SetStrokeStyleAsync("#FFA500");
                await _context!.StrokeAsync();

                // Draw event name above the line
                await _context!.SetFillStyleAsync("#FFFFFF");
                await _context!.SetTextAlignAsync(TextAlign.Center);
                await _context!.SetFontAsync("12px Arial");
                await _context!.FillTextAsync(entry.Value, x, ChartPadding - 10);

                // Draw timestamp below the line
                await _context!.SetFillStyleAsync("#FFFFFF");
                await _context!.SetTextAlignAsync(TextAlign.Center);
                await _context!.SetFontAsync("12px Arial");
                await _context!.FillTextAsync(entry.Timestamp.ToString("HH:mm"), x, Height - ChartPadding + 20);
            }
            else
            {
                // Handle numeric data points
                await _context!.SetFillStyleAsync("#FFFFFF");
                await _context!.BeginPathAsync();
                await _context!.ArcAsync(x, y, 4, 0, 2 * Math.PI);
                await _context!.FillAsync();

                if (decimal.TryParse(entry.Value, out var numericValue))
                {
                    if (previousValue != 0)
                    {
                        // Compare current value with the previous value
                        if (numericValue < previousValue)
                            lineColor = "#FF0000"; // Set line color to red if current value is lower
                        else
                            lineColor = "#00FF00"; // Set line color to green if current value is higher or equal
                    }

                    // Update previousValue for the next iteration
                    previousValue = numericValue;
                }

                if (!isFirstPoint)
                {
                    // Connect current point with the previous point using a line
                    await _context!.SetStrokeStyleAsync(lineColor);
                    await _context!.BeginPathAsync();
                    await _context!.MoveToAsync(previousX, previousY);
                    await _context!.LineToAsync(x, y);
                    await _context!.StrokeAsync();
                }

                // Update previousX and previousY for the next iteration
                previousX = x;
                previousY = y;
                isFirstPoint = false;
            }
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
        var timeSpan = timestamp - Values.Min(d => d.Timestamp);
        var totalMinutes = (float)timeSpan.TotalMinutes;
        var minutesRange = (float)(Values.Max(d => (d.Timestamp - Values.Min(t => t.Timestamp)).TotalMinutes));
        return totalMinutes / minutesRange * chartAreaWidth;
    }

    private float MapValueToY(string value, float chartAreaHeight, float minValue, float maxValue)
    {
        if (float.TryParse(value, out var numericValue))
        {
            // Calculate the Y position based on the numeric value and the available chart area height
            var valueRange = maxValue - minValue;
            return chartAreaHeight - ((numericValue - minValue) / valueRange * chartAreaHeight);
        }
        else
        {
            // Handle non-numeric values (e.g., event markers)
            return chartAreaHeight;
        }
    }

    [JSInvokable]
    public async Task HandleMouseMove(double x, double y)
    {
        _isHovering = true;
        _hoverX = x;

        // Find the closest entry to the hovered position
        var closestEntry = FindClosestEntry(x);

        // Clear canvas and redraw the chart
        await _context!.ClearRectAsync(0, 0, 800, 400);
        await DrawChart();

        if (closestEntry != null && !closestEntry.IsEvent)
        {
            // Draw value and timestamp above the red hover line
            await _context!.SetFillStyleAsync("#FFFFFF");
            await _context!.SetTextAlignAsync(TextAlign.Center);
            await _context!.SetFontAsync("12px Arial");
            await _context!.FillTextAsync(closestEntry.Value, (float)x, ChartPadding - 10);
            await _context!.FillTextAsync(closestEntry.Timestamp.ToString("HH:mm"), (float)x, Height - ChartPadding + 20);
        }
    }

    [JSInvokable]
    public async Task HandleMouseOut()
    {
        _isHovering = false;
        await DrawChart();
    }
    
    
    private LineChartValue? FindClosestEntry(double x)
    {
        var chartAreaWidth = Width - 2 * ChartPadding;
        var orderedValues = Values.OrderBy(v => v.Timestamp).ToList();

        // Find the entry with the closest X position to the hovered position
        var closestEntry = orderedValues.FirstOrDefault();
        var closestDistance = Math.Abs(MapTimestampToX(closestEntry.Timestamp, chartAreaWidth) + ChartPadding - x);

        foreach (var entry in orderedValues)
        {
            var entryX = MapTimestampToX(entry.Timestamp, chartAreaWidth) + ChartPadding;
            var distance = Math.Abs(entryX - x);

            if (distance < closestDistance)
            {
                closestEntry = entry;
                closestDistance = distance;
            }
        }

        return closestEntry;
    }
}

public record LineChartValue(DateTime Timestamp, string Value, bool IsEvent);