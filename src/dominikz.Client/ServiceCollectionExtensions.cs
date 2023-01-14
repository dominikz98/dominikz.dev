using dominikz.Client.Components.Toast;
using dominikz.Client.Utils;
using dominikz.Domain.Options;
using dominikz.Infrastructure.Clients.Api;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

namespace dominikz.Client;

public static class ServiceCollectionExtensions
{
    public static WebAssemblyHostBuilder AddOptions(this WebAssemblyHostBuilder builder)
        => builder.AddOption<ApiOptions>();

    private static WebAssemblyHostBuilder AddOption<T>(this WebAssemblyHostBuilder builder) where T : class, new()
    {
        var options = new T();
        builder.Configuration.GetSection(typeof(T).Name).Bind(options);
        builder.Services.AddSingleton(Options.Create(options));
        return builder;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddSingleton<ICredentialStorage, CredentialStorage>()
            .AddScoped<BrowserService>()
            .AddSingleton<ToastService>()
            .AddSingleton<IHttpErrorHandler, HttpToastErrorHandler>();
}