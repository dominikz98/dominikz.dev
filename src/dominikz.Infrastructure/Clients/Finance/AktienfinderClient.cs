using Polly;
using PuppeteerSharp;

namespace dominikz.Infrastructure.Clients.Finance;

public class AktienFinderClient
{
    private readonly FinanceBrowser _browser;

    public AktienFinderClient(FinanceBrowser browser)
    {
        _browser = browser;
    }

    public async Task<string?> GetLogoByISIN(string isin)
        => (await Policy
                .Handle<WaitTaskTimeoutException>()
                .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
                .ExecuteAndCaptureAsync(async () => await GetLogoByISINInternal(isin)))
            .Result;

    private async Task<string?> GetLogoByISINInternal(string isin)
    {
        var url = $"https://aktienfinder.net/aktienfinder?s={isin}";
        await using var page = await _browser.OpenPage(url);

        var logoImgSelector = "img.af-colo-sm__logo";
        var logoImgElement = await page.WaitForSelectorAsync(logoImgSelector);
        if (logoImgElement == null)
            return null;

        var logoUrl = await logoImgElement.EvaluateFunctionAsync<string>("e => e.getAttribute('src')");
        return string.IsNullOrWhiteSpace(logoUrl) 
            ? null 
            : $"https://aktienfinder.net/{logoUrl}";
    }
}