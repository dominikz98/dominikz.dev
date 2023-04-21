using AngleSharp.Html.Parser;
using Polly;
using PuppeteerSharp;

namespace dominikz.Infrastructure.Clients.Finance;

public class AktienFinderClient
{
    public async Task<string?> GetLogoByISIN(string isin)
        => (await Policy
                .Handle<WaitTaskTimeoutException>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
                .ExecuteAndCaptureAsync(async () => await GetLogoByISINInternal(isin)))
            .Result;
    
    private async Task<string?> GetLogoByISINInternal(string isin)
    {
        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

        var url = $"https://aktienfinder.net/aktienfinder?s={isin}";
        var page = await browser.NewPageAsync();
        await page.GoToAsync(url);

        var logoImgSelector = "img.af-colo-sm__logo";
        var logoImgElement = await page.WaitForSelectorAsync(logoImgSelector);
        if (logoImgElement == null)
            return null;

        var logoImgSrc = await logoImgElement.EvaluateFunctionAsync<string>("e => e.getAttribute('src')");
        if (logoImgSrc == null)
            return null;

        await browser.CloseAsync();
        return $"https://aktienfinder.net/{logoImgSrc}";
    }
}