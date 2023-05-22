using ClosedXML.Excel;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.Options;
using dominikz.Infrastructure.Extensions;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure.Excel;

public class TradingProtocolExcel : ExcelEditor
{
    private const int StartRow = 4;
    private const int StartColumn = 2;

    private static readonly DateTime NyseOpen = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 30, 0, DateTimeKind.Utc);
    private static readonly DateTime NyseClose = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 00, 0, DateTimeKind.Utc);

    public TradingProtocolExcel(IOptions<ExcelOptions> options) : base(options.Value.TradingProtocol, "Sheet1")
    {
    }

    public void Create(List<EarningCall> allCalls)
    {
        var callsPerDay = allCalls.GroupBy(x => x.UtcTimestamp.ToLocalDateTime().Date).ToList();
        var rowIdx = StartRow;

        foreach (var group in callsPerDay)
        {
            // AMC
            foreach (var call in group.Where(x => x.Time == EarningCallTime.AMC).OrderByDescending(x => x.UtcTimestamp))
                PrintCall(call, ref rowIdx);

            PrintMarketEvent(ref rowIdx);

            // BMO
            foreach (var call in group.Where(x => x.Time == EarningCallTime.BMO).OrderByDescending(x => x.UtcTimestamp))
                PrintCall(call, ref rowIdx);

            PrintSeparator(group.Key, ref rowIdx);
        }

        UpdateTimestamp();
        Save();
    }

    private void UpdateTimestamp()
        => UpdateCell(2, 13, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));

    private void PrintSeparator(DateTime date, ref int rowIdx)
    {
        var cell = Merge($"B{rowIdx}:L{rowIdx}");
        cell.Style.Fill.BackgroundColor = XLColor.BlueBell;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Value = $"Next Day ({date:dd.MM.yyyy})";
    }

    private void PrintMarketEvent(ref int rowIdx)
    {
        var cell = Merge($"B{rowIdx}:L{rowIdx}");
        cell.Style.Fill.BackgroundColor = XLColor.Apricot;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Value = $"NYSE Market ({NyseOpen.ToLocalTime():HH:mm} - {NyseClose.ToLocalTime():HH:mm})";
        rowIdx++;
    }

    private void PrintCall(EarningCall call, ref int rowIdx)
    {
        var timestamp = call.UtcTimestamp.ToLocalDateTime();
        UpdateCell(rowIdx, StartColumn, timestamp.ToString("dd.MM.yyyy")); // Date
        UpdateCell(rowIdx, StartColumn + 1, call.Company); // Name
        UpdateCell(rowIdx, StartColumn + 2, $"{timestamp:HH:mm} ({call.Time})"); // Time
        UpdateCell(rowIdx, StartColumn + 3, call.Symbol); // Symbol
        UpdateCell(rowIdx, StartColumn + 4, call.ISIN); // ISIN
        PrintFlag(rowIdx, StartColumn + 5, call.EpsFlag); // EPS
        PrintFlag(rowIdx, StartColumn + 6, call.RevenueFlag); // Revenue
        PrintPerCent(rowIdx, StartColumn + 7, call.Growth); // Growth
        PrintPerCent(rowIdx, StartColumn + 8, call.Surprise); // Surprise

        // highlight
        if (call is { EpsFlag: true, RevenueFlag: true }
            && (call.Growth > 30 || call.Surprise > 30))
            GetCellRef($"B{rowIdx}:L{rowIdx}").Style.Fill.BackgroundColor = XLColor.LightGreen;

        rowIdx++;
    }

    private void PrintFlag(int rowIdx, int columnIdx, bool? value)
    {
        if (value == null)
        {
            UpdateCell(rowIdx, columnIdx, "-");
            return;
        }

        if (value == true)
        {
            UpdateCell(rowIdx, columnIdx, "✓", cell => cell.Style.Font.FontColor = XLColor.Green);
            return;
        }

        if (value == false)
            UpdateCell(rowIdx, columnIdx, "╳", cell => cell.Style.Font.FontColor = XLColor.Red);
    }

    private void PrintPerCent(int rowIdx, int columnIdx, decimal? value)
    {
        if (value == null)
        {
            UpdateCell(rowIdx, columnIdx, "-");
            return;
        }

        UpdateCell(rowIdx, columnIdx, $"{value} %");
    }
}