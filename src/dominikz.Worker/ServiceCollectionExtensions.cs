using dominikz.Domain.Options;
using dominikz.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace dominikz.Worker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorker(this IServiceCollection services, IReadOnlyCollection<Type> workerList)
    {
        foreach (var worker in workerList)
            services.AddScoped(worker);

        return services;
    }

    public static IServiceCollection AddWorkerLogging(this IServiceCollection services)
#pragma warning disable CA1416
        => services.AddLogging(builder => builder
                .SetMinimumLevel(LogLevel.Information)
                .AddFile(options =>
                {
                    options.Directory = "logs";
                    options.UseRollingFiles = true;
                    options.RollingFileTimestampFormat = "yyyy-MM-dd";
                    options.FileExtension = "log";
                    options.FileNamePrefix = "worker";
                    options.MinimumLogLevel = LogLevel.Information;
                })
                .AddConsole()
                .AddEventLog()
#pragma warning restore CA1416
        );

    public static IServiceCollection AddWorkerOptions(this IServiceCollection services, IConfigurationRoot configuration)
        => services.Configure<ConnectionStrings>(configuration.GetSection(nameof(ConnectionStrings)))
            .Configure<ExternalUrlsOptions>(configuration.GetSection(nameof(ExternalUrlsOptions)))
            .Configure<ApiKeysOptions>(configuration.GetSection(nameof(ApiKeysOptions)));

    public static IServiceCollection AddProvider(this IServiceCollection services, IConfigurationRoot configuration)
        => services.AddContext(configuration, true)
            .AddStorage(configuration);

    public static IServiceCollection AddExternalClients(this IServiceCollection services)
        => services.AddFinancialClients()
            .AddMedlanClient()
            .AddNoobitClient()
            .AddSupermarktCheckClient();
}