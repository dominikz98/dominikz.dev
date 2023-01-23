using dominikz.Domain.Options;
using dominikz.Infrastructure.Clients;
using dominikz.Infrastructure.Clients.Api;
using dominikz.Infrastructure.Clients.JustWatch;
using dominikz.Infrastructure.Clients.Noobit;
using dominikz.Infrastructure.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiClient(this IServiceCollection services, string url)
        => services.AddSingleton<BlogEndpoints>()
            .AddSingleton<MediaEndpoints>()
            .AddSingleton<MovieEndpoints>()
            .AddSingleton<GameEndpoints>()
            .AddSingleton<BookEndpoints>()
            .AddSingleton<DownloadEndpoints>()
            .AddSingleton<SongsEndpoints>()
            .AddHttpClient<ApiClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add(ApiClient.ApiKeyHeaderName, options.Key);
            }).Services;
    
    public static IServiceCollection AddJustWatchClient(this IServiceCollection services)
        => services.AddScoped<JustWatchClient>()
            .AddHttpClient<JustWatchClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<JustWatchOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("JW DZ Client");
            }).Services;

    public static IServiceCollection AddNoobitClient(this IServiceCollection services)
        => services.AddScoped<MedlanClient>()
            .AddScoped<NoobitClient>()
            .AddHttpClient<NoobitClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<NoobitOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
            }).Services;

    public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration, bool useDevelopmentEnv)
        => services.AddDbContext<DatabaseContext>(options =>
                options.UseMySql(configuration.GetConnectionString(nameof(DatabaseContext)), MariaDbServerVersion.LatestSupportedServerVersion)
                    .EnableDetailedErrors(useDevelopmentEnv)
                    .EnableSensitiveDataLogging(useDevelopmentEnv));

    public static IServiceCollection AddStorage(this IServiceCollection services)
        => services.AddSingleton<IStorageProvider, IoStorageProvider>();
}