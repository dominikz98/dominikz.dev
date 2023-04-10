using System.Globalization;
using Blazor.Extensions;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;

namespace dominikz.Client.Components.Charts;

public partial class BarChart
{
    private Canvas2DContext? _context;
    private BECanvasComponent? _canvasRef;

    [Parameter] public List<ChartItem> Entries { get; set; } = new();
    [Parameter] public decimal MaxValue { get; set; }
    [Parameter] public int Height { get; set; } = 500;
    [Parameter] public int Width { get; set; } = 500;
    [Parameter] public string XAxisLabel { get; set; } = string.Empty;
    [Parameter] public string YAxisLabel { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender == false)
            return;

        _context = await _canvasRef.CreateCanvas2DAsync();
        await DrawBarChart();
    }

    private async Task DrawBarChart()
    {
        if (_canvasRef == null || _context == null)
            return;

        var colors = new List<string> { "#5f7d8f" };
        var barWidth = 50;
        var spacing = 5;
        var startX = 35;
        var startY = _canvasRef.Height - 15;
        var maxY = MaxValue > 0 ? MaxValue : Entries.Select(x => x.Value).Max();

        // Clear canvas
        await _context.ClearRectAsync(0, 0, _canvasRef.Width, _canvasRef.Height);

        // Draw X axis label
        await _context.SetFillStyleAsync("#FFFFFFFF");
        await _context.SetFontAsync("12px Arial");

        if (string.IsNullOrWhiteSpace(XAxisLabel) == false)
            await _context.FillTextAsync(XAxisLabel, startX + Entries.Count * (barWidth + spacing) / 2, startY + 20);

        // Draw Y axis label
        if (string.IsNullOrWhiteSpace(YAxisLabel) == false)
        {
            await _context.SaveAsync();
            await _context.TranslateAsync(20, startY / 2d);
            await _context.RotateAsync((float)(-Math.PI / 2));
            await _context.FillTextAsync(YAxisLabel, 0, 0);
            await _context.RestoreAsync();
        }

        // Draw Y axis ticks and labels
        await _context.SetFillStyleAsync("#FFFFFFFF");
        await _context.SetFontAsync("10px Arial");
        for (var i = 0; i <= 10; i++)
        {
            var y = startY - i * (startY / 10);
            await _context.BeginPathAsync();
            await _context.MoveToAsync(startX - 5, y);
            await _context.LineToAsync(startX, y);
            await _context.StrokeAsync();

            var value = Math.Round(i * maxY / 10, 2, MidpointRounding.AwayFromZero);
            await _context.FillTextAsync(value.ToString(CultureInfo.CurrentCulture), startX - 30, y + 3);
        }

        // Draw bars
        for (var i = 0; i < Entries.Count; i++)
        {
            var x = startX + i * (barWidth + spacing);
            var y = startY - (int)Math.Round(Entries[i].Value * startY / maxY);

            await _context.SetFillStyleAsync(colors[i % colors.Count]);
            await _context.FillRectAsync(x, y, barWidth, startY - y);

            // Draw bar value above the bar
            await _context.SetFillStyleAsync("#FFFFFFFF");
            await _context.SetFontAsync("12px Arial");
            var text = Entries[i].Value.ToString(CultureInfo.CurrentCulture);
            var textWidth = (await _context.MeasureTextAsync(text)).Width;
            await _context.FillTextAsync(text, x + (barWidth - textWidth) / 2, y - 10);

            // Draw label below x-axis
            await _context.SetFillStyleAsync("#FFFFFFFF");
            await _context.SetFontAsync("10px Arial");
            await _context.FillTextAsync(Entries[i].Name, x + barWidth / 2 - (await _context.MeasureTextAsync(Entries[i].Name)).Width / 2, startY + 15);
        }
    }
}