using dominikz.Client.Api;
using dominikz.Client.Components.Toast;
using dominikz.Client.Utils;
using dominikz.Domain.Options;
using Microsoft.Extensions.Options;

namespace dominikz.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services)
        => services.AddSingleton<BlogEndpoints>()
            .AddSingleton<MovieEndpoints>()
            .AddSingleton<DownloadEndpoints>()
            .AddSingleton<TradesEndpoints>()
            .AddSingleton<SongsEndpoints>()
            .AddSingleton<CookbookEndpoints>()
            .AddHttpClient<ApiClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(options.ApiUrl);
                client.DefaultRequestHeaders.Add(ApiClient.ApiKeyHeaderName, options.ApiKey);
            }).Services;
    
    public static IServiceCollection AddApiOptions(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddSingleton<IOptions<ApiOptions>>((_) => Options.Create(new ApiOptions()
        {
            ApiKey = configuration[nameof(ApiOptions.ApiKey)] ?? string.Empty,
            ApiUrl = configuration[nameof(ApiOptions.ApiUrl)] ?? string.Empty,
        }));
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddSingleton<ICredentialStorage, CredentialStorage>()
            .AddScoped<BrowserService>()
            .AddSingleton<ToastService>()
            .AddSingleton<IHttpErrorHandler, HttpToastErrorHandler>();
}