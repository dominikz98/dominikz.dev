using System.Globalization;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Charts;

public partial class BarChart
{
    private Canvas2DContext? _context;
    private BECanvasComponent? _canvasRef;

    [Parameter] public List<BarChartItem> Values { get; set; } = new();
    [Parameter] public int Height { get; set; } = 300;
    [Parameter] public int Width { get; set; } = 250;
    [Parameter] public int Minimum { get; set; } = 20;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false || Values.Count == 0)
            return;

        _context = await _canvasRef.CreateCanvas2DAsync();
        await DrawBarChart();
    }

    private async Task DrawBarChart()
    {
        await _context!.ClearRectAsync(0, 0, Width, Height);

        var maxValue = Values.Max(d => d.Value);
        var minValue = Math.Min(Values.Min(d => d.Value), Minimum); // Set the minimum value to 40
        var barCount = Values.Count;
        var barWidth = Width / barCount;
        var barHeight = Height - 20;
        var startX = 10;
        decimal previousValue = 0;

        for (var i = 0; i < barCount; i++)
        {
            var bar = Values[i];
            var barX = startX + i * barWidth;

            // Calculate the bar height based on the adjusted minimum value
            var adjustedMinValue = minValue - Minimum; // Subtract 40 from the minimum value
            var adjustedMaxValue = maxValue - Minimum; // Subtract 40 from the maximum value
            var barY = Height - Math.Max((int)(((double)bar.Value - (double)adjustedMinValue) / ((double)adjustedMaxValue - (double)adjustedMinValue) * barHeight), 10);

            await _context.BeginPathAsync();
            await _context.RectAsync(barX, barY, barWidth - 5, Height - barY);

            var color = "green";
            if (bar.Value < minValue || bar.Value < previousValue)
                color = "red";

            await _context.SetFillStyleAsync(color);
            await _context.FillAsync();

            await _context.SetFillStyleAsync("white");
            await _context.SetFontAsync("14px Arial");
            await _context.SetTextAlignAsync(TextAlign.Center);
            await _context.SetTextBaselineAsync(TextBaseline.Top);
            await _context.FillTextAsync(bar.Value.ToString(), barX + barWidth / 2, barY + 5);

            await _context.SetFillStyleAsync("white");
            await _context.SetFontAsync("12px Arial");
            await _context.SetTextAlignAsync(TextAlign.Center);
            await _context.SetTextBaselineAsync(TextBaseline.Bottom);
            await _context.FillTextAsync(bar.Text, barX + barWidth / 2, Height - 5);

            previousValue = bar.Value;
        }
    }
}

public record BarChartItem(string Text, decimal Value);