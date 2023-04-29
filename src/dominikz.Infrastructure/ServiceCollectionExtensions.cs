using dominikz.Domain.Options;
using dominikz.Infrastructure.Clients;
using dominikz.Infrastructure.Clients.Finance;
using dominikz.Infrastructure.Clients.JustWatch;
using dominikz.Infrastructure.Clients.Noobit;
using dominikz.Infrastructure.Clients.SupermarktCheck;
using dominikz.Infrastructure.Provider.Database;
using dominikz.Infrastructure.Provider.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace dominikz.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFinancialClients(this IServiceCollection services)
        => services.AddScoped<OnVistaClient>()
            .AddScoped<EarningsWhispersClient>()
            .AddScoped<FinnhubClient>()
            .AddHttpClient<FinnhubClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ExternalUrlsOptions>>();
                client.BaseAddress = new Uri(options.Value.Finnhub);
            })
            .Services;

    public static IServiceCollection AddJustWatchClient(this IServiceCollection services)
        => services.AddScoped<JustWatchClient>()
            .AddHttpClient<JustWatchClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ExternalUrlsOptions>>();
                client.BaseAddress = new Uri(options.Value.JustWatch);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("JW DZ Client");
            }).Services;

    public static IServiceCollection AddMedlanClient(this IServiceCollection services)
        => services.AddScoped<MedlanClient>();

    public static IServiceCollection AddNoobitClient(this IServiceCollection services)
        => services.AddScoped<NoobitClient>()
            .AddHttpClient<NoobitClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ExternalUrlsOptions>>();
                client.BaseAddress = new Uri(options.Value.Noobit);
            }).Services;

    public static IServiceCollection AddSupermarktCheckClient(this IServiceCollection services)
        => services.AddScoped<SupermarktCheckClient>();

    public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration, bool useDevelopmentEnv)
        => services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlite(configuration.GetConnectionString(nameof(DatabaseContext)))
                .EnableDetailedErrors(useDevelopmentEnv)
                .EnableSensitiveDataLogging(useDevelopmentEnv));

    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
        => services.AddSingleton<IStorageProvider>(_ =>
            new StorageProvider(configuration.GetConnectionString(nameof(StorageProvider))));
}