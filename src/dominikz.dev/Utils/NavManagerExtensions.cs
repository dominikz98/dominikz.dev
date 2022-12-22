using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace dominikz.dev.Utils;

public static class NavManagerExtensions
{
    public static bool TryGetQueryByKey<T>(this NavigationManager navigationManager, string key, out T? value)
    {
        value = default;

        var query = navigationManager!.ToAbsoluteUri(navigationManager.Uri).Query;
        var qParams = QueryHelpers.ParseQuery(query);

        if (qParams.TryGetValue(key, out var valueRaw) == false)
            return false;

        if (typeof(T) == typeof(string))
            value = (T)Convert.ChangeType(valueRaw.ToString(), typeof(T));

        else if (typeof(T).IsEnum)
        {
            if (Enum.TryParse(typeof(T), valueRaw.ToString(), true, out var enumValue) == false)
                return false;

            value = (T)enumValue;
        }
        else
            throw new NotImplementedException();

        return true;
    }

    public static void AttachOrUpdateQuery(this NavigationManager navigationManager, string key, string? value)
    {
        var url = navigationManager!.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);
        var query = navigationManager!.ToAbsoluteUri(navigationManager.Uri).Query;
        var qParams = QueryHelpers.ParseQuery(query);

        if (qParams.TryGetValue(key, out var _) == false)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            
            // attach
            var newParams = new Dictionary<string, string>() { { key, value.ToLower() } };
            var newUrl = QueryHelpers.AddQueryString(navigationManager.Uri, newParams);
            navigationManager.NavigateTo(newUrl);
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
            // remove
            qParams.Remove(key);
        else
            // update
            qParams[key] = value.ToLower();

        var updatedParams = qParams.ToDictionary(x => x.Key, x => x.Value.ToString());
        var updatedUrl = QueryHelpers.AddQueryString(url, updatedParams);
        navigationManager.NavigateTo(updatedUrl);
    }
}