using System.Runtime.InteropServices;
using System.Security.Cryptography;
using dominikz.Domain.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace dominikz.Application.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ApiKeyAttribute : Attribute, IAuthorizationFilter
{
    public bool RequiresMasterKey { get; set; }
    public const string ApiKeyHeaderName = "x-api-key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // get api key from header or query parameter
        if (TryGetSubmittedApiKey(context.HttpContext, out var submittedApiKey) == false)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // check client api key
        var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<ApiKeyOptions>>();
        var isClientValid = IsApiKeyValid(options.Value.Client, submittedApiKey!);
        if (RequiresMasterKey == false && isClientValid)
            return;

        // check master api key
        var isMasterValid = IsApiKeyValid(options.Value.Master, submittedApiKey!);
        if (isMasterValid)
            return;

        context.Result = new UnauthorizedResult();
    }

    private static bool TryGetSubmittedApiKey(HttpContext context, out string? submittedApiKey)
    {
        submittedApiKey = context.Request.Headers[ApiKeyHeaderName];
        if (string.IsNullOrWhiteSpace(submittedApiKey) == false)
            return true;

        if (context.Request.Query.TryGetValue(ApiKeyHeaderName, out var claim)
            && string.IsNullOrWhiteSpace(claim) == false)
        {
            submittedApiKey = claim;
            return true;
        }

        return false;
    }

    private static bool IsApiKeyValid(string expected, string submitted)
    {
        var submittedKey = MemoryMarshal.Cast<char, byte>(submitted.AsSpan());
        var expectedKey = MemoryMarshal.Cast<char, byte>(expected.AsSpan());
        return CryptographicOperations.FixedTimeEquals(expectedKey, submittedKey);
    }
}