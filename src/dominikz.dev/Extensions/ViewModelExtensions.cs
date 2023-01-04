using System.Web;
using dominikz.dev.Endpoints;
using dominikz.shared.Contracts;

namespace dominikz.dev.Extensions;

public static class ViewModelExtensions
{
    public static void AttachApiKey<T>(this IEnumerable<T> source, string value) where T : IHasImageUrl
    {
        foreach (var item in source)
            item.AttachApiKey(value);
    }
    
    public static void AttachApiKey(this IHasImageUrl vm, string value)
    {
        if (string.IsNullOrWhiteSpace(vm?.ImageUrl))
            return;

        var builder = new UriBuilder(vm.ImageUrl);
        var query = HttpUtility.ParseQueryString(builder.Query);
        query.Add(ApiClient.ApiKeyHeaderName, value);
        builder.Query = query.ToString();
        vm.ImageUrl = builder.ToString();
    }
}