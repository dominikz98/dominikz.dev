using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace dominikz.dev.Utils;

public static class NavigationManagerExtensions
{
    public static string? GetQueryParamByKey(this NavigationManager navigationManager, string key)
    {
        var query = navigationManager.ToAbsoluteUri(navigationManager.Uri).Query;
        var qParams = QueryHelpers.ParseQuery(query);

        if (qParams.TryGetValue(key, out var valueRaw) == false
            || string.IsNullOrWhiteSpace(valueRaw))
            return null;

        return valueRaw.ToString();
    }

    public static T? GetQueryParamByKey<T>(this NavigationManager navigationManager, string key, bool handleDefaultAsNull = true) where T : struct
    {
        var value = GetQueryParamByKey(navigationManager, key);
        if (value is null)
            return null;

        if (typeof(T) == typeof(string))
            return (T)Convert.ChangeType(value, typeof(T));

        if (typeof(T).IsEnum)
        {
            if (Enum.TryParse(typeof(T), value, true, out var enumValue) == false)
                return null;

            if (handleDefaultAsNull && enumValue.Equals(default(T)))
                return null;

            return (T)enumValue;
        }

        if (typeof(T) == typeof(int))
        {
            if (int.TryParse(value, out var intValue) == false)
                return null;

            if (handleDefaultAsNull && intValue.Equals(default(T)))
                return null;

            return (T)(object)intValue;
        }

        throw new NotImplementedException();
    }

    public static string AttachOrUpdateRawQueryParam(this NavigationManager navigationManager, string key, string? value, bool handleDefaultAsNull = true, bool navigate = true)
    {
        var url = navigationManager.ToAbsoluteUri(navigationManager.Uri).GetLeftPart(UriPartial.Path);
        var query = navigationManager.ToAbsoluteUri(navigationManager.Uri).Query;
        var qParams = QueryHelpers.ParseQuery(query);

        if (qParams.TryGetValue(key, out _) == false)
        {
            if (value is null || (handleDefaultAsNull && value == string.Empty))
                return url;

            // attach
            var newParams = new Dictionary<string, string>() { { key, value.ToLower() } };
            var newUrl = QueryHelpers.AddQueryString(navigationManager.Uri, newParams);
            navigationManager.NavigateTo(newUrl);
            return url;
        }

        if (value is null || (handleDefaultAsNull && value == string.Empty))
            // remove
            qParams.Remove(key);
        else
            // update
            qParams[key] = value.ToLower();

        var updatedParams = qParams.ToDictionary(x => x.Key, x => x.Value.ToString());
        var updatedUrl = QueryHelpers.AddQueryString(url, updatedParams);

        if (navigate)
            navigationManager.NavigateTo(updatedUrl);

        return updatedUrl;
    }

    public static string AttachOrUpdateQueryParam<T>(this NavigationManager navigationManager, string key, T? value, bool handleDefaultAsNull = true, bool navigate = true) where T : struct
        => AttachOrUpdateRawQueryParam(navigationManager, key, value?.ToString(), handleDefaultAsNull, navigate);
}