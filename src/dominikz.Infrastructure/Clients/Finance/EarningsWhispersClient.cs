using System.Globalization;
using Polly;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace dominikz.Infrastructure.Clients.Finance;

public class EarningsWhispersClient
{
    private readonly FinanceBrowser _browser;

    public EarningsWhispersClient(FinanceBrowser browser)
    {
        _browser = browser;
    }

    public async Task<IReadOnlyCollection<EwCall>> GetEarningsCallsOfToday()
        => (await Policy
                .Handle<WaitTaskTimeoutException>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
                .ExecuteAndCaptureAsync(async () => await GetEarningsCallsOfTodayInternal()))
            .Result;

    private async Task<IReadOnlyCollection<EwCall>> GetEarningsCallsOfTodayInternal()
    {
        await using var page = await _browser.OpenPage($"https://www.earningswhispers.com/calendar");

        // switch to list view
        await page.EvaluateExpressionAsync("gotolistview()");
        await page.WaitForNavigationAsync();

        // switch to show all
        var showAllButton = await page.QuerySelectorAsync(".switch-label.switch-label-both");
        await showAllButton.ClickAsync(new ClickOptions() { Button = MouseButton.Left, Delay = 5 });
        await page.EvaluateExpressionAsync("hideorshow()");

        var result = new List<EwCall>();

        var calendar = await CrawListById(page, "epscalendar");
        result.AddRange(calendar);

        // click show more button if required
        var showMoreButton = await page.QuerySelectorAsync("#showmore");
        if (showMoreButton == null)
            return result;

        await page.EvaluateExpressionAsync("getmore()");
        var moreCalendar = await CrawListById(page, "morecalendar");
        result.AddRange(moreCalendar);

        return result;
    }

    private async Task<IReadOnlyCollection<EwCall>> CrawListById(IPage page, string id)
    {
        var result = new List<EwCall>();
        var epsCalendar = await page.QuerySelectorAsync($"ul#{id}");
        var calendarEntries = await epsCalendar.QuerySelectorAllAsync("li:not(#calhead)");
        foreach (var entry in calendarEntries)
        {
            var confirmed = await entry.QuerySelectorAsync(".confirm.icon-check") != null;
            if (confirmed == false)
                continue;

            var earning = new EwCall();
            await AttachGeneralInfo(earning, page, entry);
            await AttachTimeInfo(earning, page, entry);
            result.Add(earning);
        }

        return result;
    }

    private async Task AttachGeneralInfo(EwCall callVm, IPage page, IElementHandle entry)
    {
        var companyElement = await entry.QuerySelectorAsync(".company");
        var tickerElement = await entry.QuerySelectorAsync(".ticker");
        callVm.Name = await page.EvaluateFunctionAsync<string>("e => e.textContent", companyElement);
        callVm.Symbol = await page.EvaluateFunctionAsync<string>("e => e.textContent", tickerElement);

        var growthElement = await entry.QuerySelectorAsync(".revgrowthfull");
        if (growthElement != null)
        {
            var growthRaw = await page.EvaluateFunctionAsync<string>("e => e.textContent", growthElement);
            if (string.IsNullOrWhiteSpace(growthRaw) == false && decimal.TryParse(growthRaw.Substring(0, growthRaw.Length - 1), out var growth))
                callVm.Growth = growth;
        }

        var surpriseElement = await entry.QuerySelectorAsync(".epssurpfull");
        if (surpriseElement != null)
        {
            var surpriseRaw = await page.EvaluateFunctionAsync<string>("e => e.textContent", surpriseElement);
            if (string.IsNullOrWhiteSpace(surpriseRaw) == false && decimal.TryParse(surpriseRaw.Substring(0, surpriseRaw.Length - 1), out var surprise))
                callVm.Surprise = surprise;
        }
    }

    private async Task AttachTimeInfo(EwCall callVm, IPage page, IElementHandle entry)
    {
        var element = await entry.QuerySelectorAsync(".time");
        if (element == null)
            return;

        var rawTime = await page.EvaluateFunctionAsync<string>("e => e.textContent", element);
        if (rawTime.Contains(':') == false)
            return;

        var easternTime = DateTime.ParseExact(rawTime.Remove(rawTime.Length - 3, 3), "h:mm tt", CultureInfo.InvariantCulture);
        var easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(easternTime, easternZone);
        callVm.Release = TimeOnly.FromDateTime(utcTime);
    }
}

public class EwCall
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public TimeOnly? Release { get; set; }
    public decimal? Growth { get; set; }
    public decimal? Surprise { get; set; }
}