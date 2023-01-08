using System.Web;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using dominikz.shared.Contracts;
using dominikz.shared.ViewModels;

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
    
    public static EditPersonWrapper Wrap(this EditPersonVm vm)
        => new()
        {
            Id = vm.Id,
            Tracked = vm.Tracked,
            Category = vm.Category,
            Name = vm.Name
        };
}