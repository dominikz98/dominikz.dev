using dominikz.Infrastructure.Clients.Noobit;
using dominikz.Infrastructure.Provider.Database;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Tests;

public class NoobitClientTests
{
    [Fact]
    public async Task Test()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite("Data Source= /home/dominikzettl/RiderProjects/dominikz.dev/src/dominikz.Migrations/dominikz.db")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        var context = new DatabaseContext(options);
        
        var client = new NoobitClient(new HttpClient()
        {
            BaseAddress = new Uri("https://www.noobit.dev/")
        });

        var results = await client.GetArticles(default);
        Assert.NotEmpty(results);
        Assert.NotEmpty(results.First().Title);
        Assert.NotEqual(default, results.First().Date);

        await context.AddRangeAsync(results);
        await context.SaveChangesAsync();

    }
}