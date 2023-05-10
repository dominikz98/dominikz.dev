using dominikz.Domain.Options;
using dominikz.Infrastructure;
using dominikz.Infrastructure.Clients;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dominikz.Worker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkerOptions(this IServiceCollection services, IConfigurationRoot configuration)
        => services.Configure<ConnectionStrings>(configuration.GetSection(nameof(ConnectionStrings)))
            .Configure<EmailOptions>(configuration.GetSection(nameof(EmailOptions)))
            .Configure<ExternalUrlsOptions>(configuration.GetSection(nameof(ExternalUrlsOptions)))
            .Configure<ApiKeysOptions>(configuration.GetSection(nameof(ApiKeysOptions)));

    public static IServiceCollection AddProvider(this IServiceCollection services, IConfigurationRoot configuration)
        => services.AddContext(configuration, true)
            .AddStorage(configuration)
            .AddScoped<EmailClient>();

    public static IServiceCollection AddExternalClients(this IServiceCollection services)
        => services.AddFinancialClients()
            .AddMedlanClient()
            .AddNoobitClient()
            .AddSupermarktCheckClient();
}