using dominikz.dev.Components.Toast;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Utils;

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
        => services.AddSingleton<AuthService>()
            .AddScoped<BrowserService>()
            .AddSingleton<ToastService>();

    public static IServiceCollection AddEndpoints(this IServiceCollection services)
        => services.AddScoped<BlogEndpoints>()
            .AddScoped<MediaEndpoints>()
            .AddScoped<MovieEndpoints>()
            .AddScoped<GameEndpoints>()
            .AddScoped<BookEndpoints>()
            .AddSingleton<AuthEndpoints>()
            .AddScoped<MusicEndpoints>()
            .AddHttpClient<ApiClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
                client.BaseAddress = new Uri(options.Url);
                client.DefaultRequestHeaders.Add(ApiClient.ApiKeyHeaderName, options.Key);
            })
            .Services;
}