using ClosedXML.Excel;
using dominikz.Domain.Enums;
using dominikz.Domain.Models;
using dominikz.Domain.Options;
using dominikz.Infrastructure.Extensions;
using Microsoft.Extensions.Options;
using PuppeteerSharp.Input;

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
        var cell = Merge($"B{rowIdx}:J{rowIdx}");
        cell.Style.Fill.BackgroundColor = XLColor.BlueBell;
        cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        cell.Value = $"Next Day ({date:dd.MM.yyyy})";
    }

    private void PrintMarketEvent(ref int rowIdx)
    {
        var cell = Merge($"B{rowIdx}:J{rowIdx}");
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
        UpdateCell(rowIdx, StartColumn + 5, FlatToString(call.EpsFlag)); // EPS
        UpdateCell(rowIdx, StartColumn + 6, FlatToString(call.NetIncomeFlag)); // NET
        rowIdx++;
    }

    private string FlatToString(bool? value)
        => value switch
        {
            true => "✅",
            false => "❌",
            _ => "-"
        };
}