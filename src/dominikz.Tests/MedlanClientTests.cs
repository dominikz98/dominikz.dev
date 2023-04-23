using dominikz.Domain.Options;
using dominikz.Infrastructure.Clients;
using Microsoft.Extensions.Options;

namespace dominikz.Tests;

public class MedlanClientTests
{
    [Fact]
    public async Task TestArchivCrawl()
    {
        var client = new MedlanClient(Options.Create(new ExternalUrlsOptions()
        {
            Medlan = "https://www.medlan.de/"
        }));

        var results = await client.GetArticles(default);
        Assert.NotEmpty(results);
        Assert.NotEmpty(results.First().Title);
        Assert.NotEqual(default, results.First().Date);
    }
}