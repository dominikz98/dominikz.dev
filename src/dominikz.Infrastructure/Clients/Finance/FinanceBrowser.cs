// using PuppeteerSharp;
//
// namespace dominikz.Infrastructure.Clients.Finance;
//
// public class FinanceBrowser
// {
//     private IBrowser? _browser;
//
//     private async Task InitBrowserIfRequired()
//     {
//         if (_browser != null)
//             return;
//
//         await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
//         _browser = await Puppeteer.LaunchAsync(new LaunchOptions
//         {
//             Headless = true
//         });
//     }
//
//     public async Task<IPage> OpenPage(string url, CookieParam? cookie = null)
//     {
//         await InitBrowserIfRequired();
//         var page = await _browser!.NewPageAsync();
//         if (cookie != null)
//             await page.SetCookieAsync(cookie);
//
//         await page.GoToAsync(url, waitUntil: WaitUntilNavigation.Load);
//         return page;
//     }
// }