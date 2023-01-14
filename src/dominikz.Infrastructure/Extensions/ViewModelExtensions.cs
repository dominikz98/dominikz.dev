using System.Web;
using dominikz.Domain.Contracts;
using dominikz.Infrastructure.Clients.Api;

namespace dominikz.Infrastructure.Extensions;

public static class ViewModelExtensions
{
    public static void AttachApiKey<T>(this IEnumerable<T> source, string value) where T : IHasImageUrl
    {
        foreach (var item in source)
            item.AttachApiKey(value);
    }
    
    public static void AttachApiKey(this IHasImageUrl vm, string value)
    {
        if (string.IsNullOrWhiteSpace(vm.ImageUrl))
            return;

        vm.ImageUrl = AttachApiKey(vm.ImageUrl, value);
    }
    
    public static string AttachApiKey(string source, string key)
    {
        var builder = new UriBuilder(source);
        var query = HttpUtility.ParseQueryString(builder.Query);
        query.Add(ApiClient.ApiKeyHeaderName, key);
        builder.Query = query.ToString();
        return builder.ToString();
    }
}