using System.Web;
using Polly;
using PuppeteerSharp;

namespace dominikz.Infrastructure.Clients.Finance;

public class FinanzenNetClient
{
    public async Task<string?> GetISINBySymbolAndKeyword(string symbol, string keyword)
        => (await Policy
                .Handle<WaitTaskTimeoutException>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
                .ExecuteAndCaptureAsync(async () => await GetISINBySymbolAndKeywordInternal(symbol, keyword)))
            .Result;

    private async Task<string?> GetISINBySymbolAndKeywordInternal(string symbol, string keyword)
    {
        var cleanedKeyword = keyword.Replace(".", "")
            .Replace(",", "");

        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

        var url = $"https://www.finanzen.net/suchergebnis.asp?_search={HttpUtility.UrlEncode(cleanedKeyword)}";
        var page = await browser.NewPageAsync();
        await page.GoToAsync(url);

        // Wait for the page to load
        var pageContent = await page.GetContentAsync();
        var noResultsFound = pageContent.Contains("Ihre Suchanfrage hat in unserer Wertpapierdatenbank kein Ergebnis geliefert. Wir zeigen Ihnen daher das Suchergebnis einer Volltextsuche über finanzen.net.");
        if (noResultsFound)
            return null;

        var specificationRequired = pageContent.Contains("Ihre Suchanfrage war nicht eindeutig. Bitte wählen Sie aus den untenstehenden Möglichkeiten.");
        if (specificationRequired == false)
            // unknown state
            return null;

        var tables = await page.QuerySelectorAllAsync("table");
        foreach (var table in tables)
        {
            var headers = await table.QuerySelectorAllAsync("thead > tr > th");
            var hasNameColumn = false;
            var hasISINColumn = false;
            foreach (var header in headers)
            {
                var textContent = await header.EvaluateFunctionAsync<string>("(element) => element.textContent");
                if (textContent.Contains("Name"))
                    hasNameColumn = true;
                else if (textContent.Contains("ISIN / Symbol"))
                    hasISINColumn = true;
            }

            if (!hasNameColumn || !hasISINColumn)
                continue;

            var rows = await table.QuerySelectorAllAsync("tbody tr");
            foreach (var row in rows)
            {
                var isin = await row.QuerySelectorAsync("td:nth-child(2)").EvaluateFunctionAsync<string>("(element) => element.textContent");
                var link = await row.QuerySelectorAsync("td:nth-child(1) > a").EvaluateFunctionAsync<string>("(element) => element.getAttribute('href')");

                if (isin.Equals(symbol, StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                var result = await ExtractISINFromStockPage($"https://www.finanzen.net{link}");
                if (string.IsNullOrWhiteSpace(result))
                    continue;

                return result;
            }
        }

        return null;
    }

    private static async Task<string?> ExtractISINFromStockPage(string url)
        => (await Policy
                .Handle<WaitTaskTimeoutException>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
                .ExecuteAndCaptureAsync(async () =>
                {
                    await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
                    await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

                    var page = await browser.NewPageAsync();
                    await page.GoToAsync(url);

                    return await page.EvaluateExpressionAsync<string>(@"(() => {
                const badges = Array.from(document.querySelectorAll('.badge-bar .badge'));
                const isinBadge = badges.find(badge => badge.querySelector('.badge__key').textContent === 'ISIN');
                return isinBadge.querySelector('.badge__value').textContent;})()");
                }))
            .Result;
}