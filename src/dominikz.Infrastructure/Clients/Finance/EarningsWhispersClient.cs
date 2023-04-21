using System.Globalization;
using dominikz.Domain.Enums.Trades;
using dominikz.Domain.Models;
using Polly;
using PuppeteerSharp;

namespace dominikz.Infrastructure.Clients.Finance;

public class EarningsWhispersClient
{
    public async Task<IReadOnlyCollection<EarningCall>> GetEarningsCallsOfToday()
        => (await Policy
                .Handle<WaitTaskTimeoutException>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
                .ExecuteAndCaptureAsync(async () => await GetEarningsCallsOfTodayInternal()))
            .Result;

    private async Task<IReadOnlyCollection<EarningCall>> GetEarningsCallsOfTodayInternal()
    {
        // Launch a new instance of the Chromium browser
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        var page = await browser.NewPageAsync();
        await page.GoToAsync($"https://www.earningswhispers.com/calendar");

        var showMoreButton = await page.QuerySelectorAsync("#showmore");
        if (showMoreButton != null)
            await showMoreButton.ClickAsync();

        var result = new List<EarningCall>();
        var epsCalendar = await page.QuerySelectorAsync("ul#epscalendar");
        var calendarEntries = await epsCalendar.QuerySelectorAllAsync("li:not(#calhead)");
        foreach (var entry in calendarEntries)
        {
            var confirmed = await entry.QuerySelectorAsync(".confirm.icon-check") != null;
            if (confirmed == false)
                continue;

            var earning = new EarningCall
            {
                Sources = InformationSource.EarningsWhispers,
                Date = DateOnly.FromDateTime(DateTime.Now)
            };

            await AttachGeneralInfo(earning, page, entry);
            await AttachTimeInfo(earning, page, entry);
            result.Add(earning);
        }

        await browser.CloseAsync();
        return result;
    }


    private async Task AttachGeneralInfo(EarningCall callVm, IPage page, IElementHandle entry)
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

    private async Task AttachTimeInfo(EarningCall callVm, IPage page, IElementHandle entry)
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