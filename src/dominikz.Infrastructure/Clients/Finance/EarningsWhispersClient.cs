using System.Globalization;
using dominikz.Domain.Options;
using Microsoft.Extensions.Options;
using Polly;
using PuppeteerSharp;
using PuppeteerSharp.Input;

namespace dominikz.Infrastructure.Clients.Finance;

public class EarningsWhispersClient
{
    private readonly IOptions<ExternalUrlsOptions> _options;

    public EarningsWhispersClient(IOptions<ExternalUrlsOptions> options)
    {
        _options = options;
    }

    public async Task<IReadOnlyCollection<EwCall>> GetEarningsCallsOfToday()
        => (await Policy
                .Handle<WaitTaskTimeoutException>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
                .ExecuteAndCaptureAsync(async () => await GetEarningsCallsOfTodayInternal()))
            .Result;

    private async Task<IReadOnlyCollection<EwCall>> GetEarningsCallsOfTodayInternal()
    {
        using var fetcher = new BrowserFetcher();
        await fetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });
        await using var page = await browser!.NewPageAsync();
        await page.GoToAsync($"{_options.Value.EarningsWhispers}calendar", waitUntil: WaitUntilNavigation.Load);

        // Wait for any network activity to finish
        await page.WaitForNetworkIdleAsync();

        // switch to list view
        await page.EvaluateExpressionAsync("gotolistview()");
        await page.WaitForNavigationAsync();

        // switch to show all
        var showAllButton = await page.QuerySelectorAsync(".switch-label.switch-label-both");
        await showAllButton.ClickAsync(new ClickOptions() { Button = MouseButton.Left, Delay = 5 });
        await page.EvaluateExpressionAsync("hideorshow()");
        // await Wait1Sec(page);

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

        return result.DistinctBy(x => x.Symbol).OrderBy(x => x.Symbol)
            .ToList();
    }

    private async Task Wait1Sec(IPage page)
    {
        try
        {
            await page.WaitForNetworkIdleAsync(new WaitForNetworkIdleOptions() { IdleTime = 1000, Timeout = 1000 });
        }
        catch (TimeoutException)
        {
        }
    }

    private async Task<IReadOnlyCollection<EwCall>> CrawListById(IPage page, string id)
    {
        var result = new List<EwCall>();
        await page.WaitForSelectorAsync($"ul#{id}");
        var epsCalendar = await page.QuerySelectorAsync($"ul#{id}");

        // Wait for the selector to appear on the page
        await page.WaitForSelectorAsync("li:not(#calhead)");
        var calendarEntries = await epsCalendar.QuerySelectorAllAsync("li:not(#calhead)");
        foreach (var entry in calendarEntries)
        {
            var confirmed = await entry.QuerySelectorAsync(".confirm.icon-check") != null;
            if (confirmed == false)
                continue;

            var earning = new EwCall();
            await AttachSymbol(earning, page, entry);
            await TryAttachEpsResult(earning, page, entry);
            await TryAttachTimeInfo(earning, page, entry);
            result.Add(earning);
        }

        return result;
    }

    private async Task AttachSymbol(EwCall callVm, IPage page, IElementHandle entry)
    {
        var tickerElement = await entry.QuerySelectorAsync(".ticker");
        callVm.Symbol = await page.EvaluateFunctionAsync<string>("e => e.textContent", tickerElement);
    }

    private async Task TryAttachEpsResult(EwCall callVm, IPage page, IElementHandle entry)
    {
        var negative = false;
        var element = await entry.QuerySelectorAsync(".actual.green");
        if (element == null)
            negative = true;

        element ??= await entry.QuerySelectorAsync(".actual.red");
        if (element == null)
            return;

        var rawValue = await page.EvaluateFunctionAsync<string>("e => e.textContent", element);
        if (string.IsNullOrWhiteSpace(rawValue))
            return;

        rawValue = rawValue.Replace("(", "")
            .Replace(")", "")
            .Replace("$", "");

        if (decimal.TryParse(rawValue, CultureInfo.InvariantCulture, out var value) == false)
            return;

        callVm.Eps = value * (negative ? -1 : 1);
    }

    private async Task TryAttachTimeInfo(EwCall callVm, IPage page, IElementHandle entry)
    {
        var element = await entry.QuerySelectorAsync(".time");
        if (element == null)
            return;

        var rawTime = await page.EvaluateFunctionAsync<string>("e => e.textContent", element);
        if (rawTime.Contains(":") == false)
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
    public TimeOnly? Release { get; set; }
    public decimal? Eps { get; set; }
}