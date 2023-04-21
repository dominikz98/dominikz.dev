using System.Globalization;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using Polly;
using PuppeteerSharp;

namespace dominikz.Infrastructure.Clients.SupermarktCheck;

public class SupermarktCheckClient
{
    private static readonly CultureInfo Culture = new("De-de");

    public async Task<ProductVm?> GetProductById(int productId, CancellationToken cancellationToken)
    {
        try
        {
            return (await Policy
                    .Handle<WaitTaskTimeoutException>()
                    .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(1))
                    .ExecuteAndCaptureAsync(async () =>
                    {
                        var page = await GetPageFromWebsite(productId, cancellationToken);
                        var product = ParseToVm(productId, page);
                        return product;
                        ;
                    }))
                .Result;
        }
        catch (InvalidCastException)
        {
            return null;
        }
    }

    private async Task<IDocument> GetPageFromWebsite(int productId, CancellationToken cancellationToken)
    {
        // Prefetch revision
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

        // Launch a headless browser
        var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        // Create a new page in the browser
        var page = await browser.NewPageAsync();

        // Navigate to the URL and wait for any JavaScript redirects to complete
        var url = $"https://www.supermarktcheck.de/product/{productId}";
        await page.GoToAsync(url, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } });

        // Get the page content as an HTML string
        string html = await page.GetContentAsync();

        // Create a new AngleSharp document from the HTML string
        var config = Configuration.Default;
        var context = BrowsingContext.New(config);
        var document = await context.OpenAsync(req => req.Content(html), cancellationToken);

        // Close the headless browser
        await browser.CloseAsync();
        return document;
    }

    private ProductVm ParseToVm(int id, IParentNode document)
    {
        var name = document.QuerySelector("h1.details")?.TextContent;
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidCastException("Cant cast name!");

        var prices = GetPrices(document, true);
        if (prices.Count == 0)
            prices = GetPrices(document, false);

        var nutritionalValues = GetNutritionalValues(document);

        return new ProductVm
        {
            Id = id,
            Name = name,
            NutritionalValues = nutritionalValues,
            Prices = prices
        };
    }

    private IReadOnlyCollection<ProductPriceVm> GetPrices(IParentNode document, bool ignoreObsolete)
    {
        var rows = document.QuerySelector("#preise")
            ?.QuerySelector("tbody")
            ?.QuerySelectorAll("tr");

        if (rows == null || rows.Length == 0)
            throw new InvalidCastException("Cant cast prices!");

        var result = new List<ProductPriceVm>();
        foreach (var row in rows)
        {
            var cells = row.QuerySelectorAll("td").ToList();

            if (cells.Count is < 4 or > 5)
                continue;

            if (cells.Count == 5)
                cells.RemoveAt(1);

            if (ignoreObsolete && cells[3].TextContent.Contains("abgelaufen"))
                continue;

            var store = cells[0].QuerySelectorAll("strong").FirstOrDefault()?.TextContent;
            if (string.IsNullOrEmpty(store))
                throw new InvalidCastException("Cant cast price store!");

            var priceMatch = Regex.Match(cells[1].TextContent, @"([-+]?[0-9]*\,?[0-9]+)");
            if (priceMatch.Success == false || decimal.TryParse(priceMatch.Value, Culture, out var price) == false)
                throw new InvalidCastException("Cant cast price per unit!");

            result.Add(new ProductPriceVm()
            {
                Store = store,
                Price = price,
            });
        }

        return result;
    }

    private IReadOnlyCollection<ProductNutritionVm> GetNutritionalValues(IParentNode document)
    {
        var rows = document.QuerySelector("#naehrwerte")
            ?.QuerySelector("tbody")
            ?.QuerySelectorAll("tr");

        var result = new List<ProductNutritionVm>();
        if (rows == null || rows.Length == 0)
            return result;

        foreach (var row in rows)
        {
            var cells = row.QuerySelectorAll("td");

            if (cells.Length < 3)
                throw new InvalidCastException("Cant cast nutritional values!");

            var name = cells[0].TextContent;
            if (string.IsNullOrEmpty(name))
                throw new InvalidCastException("Cant cast nutrition name!");

            var valueParts = cells[^2].TextContent.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (valueParts.Length != 2 || decimal.TryParse(valueParts[0], Culture, out var value) == false)
                throw new InvalidCastException("Cant cast nutrition value!");

            if (Enum.TryParse<NutritionUnit>(valueParts[1], true, out var unit) == false)
                unit = NutritionUnit.Unknown;

            result.Add(new ProductNutritionVm()
            {
                Name = name,
                Value = value,
                Unit = unit
            });
        }

        return result;
    }
}