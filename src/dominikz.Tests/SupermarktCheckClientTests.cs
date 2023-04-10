using dominikz.Infrastructure.Clients.SupermarktCheck;

namespace dominikz.Tests;

public class SupermarktCheckClientTests
{
    [Theory]
    [InlineData(72344)] // Kopfsalat
    public async Task Test(int id)
    {
        var client = new SupermarktCheckClient();
        var result = await client.GetProductById(id, default);
        
        Assert.NotNull(result);
        Assert.NotEmpty(result.Name);
        Assert.NotEmpty(result.NutritionalValues);
        Assert.NotEmpty(result.Prices);
    }
}