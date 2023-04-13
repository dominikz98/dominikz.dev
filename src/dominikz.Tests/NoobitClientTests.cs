using dominikz.Infrastructure.Clients.Noobit;

namespace dominikz.Tests;

public class NoobitClientTests
{
    [Fact]
    public async Task Test()
    {
        var client = new NoobitClient(new HttpClient()
        {
            BaseAddress = new Uri("https://www.noobit.dev/")
        });

        var results = await client.GetArticles(default);
        Assert.NotEmpty(results);
        Assert.NotEmpty(results.First().Title);
        Assert.NotEqual(default, results.First().Date);
    }
}