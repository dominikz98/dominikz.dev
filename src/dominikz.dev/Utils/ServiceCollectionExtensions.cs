using dominikz.dev.Components.Toast;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

namespace dominikz.dev.Utils;

public static class ServiceCollectionExtensions
{
    public static WebAssemblyHostBuilder AddOptions(this WebAssemblyHostBuilder builder)
    {
        var externalUrls = new ExternalUrls()
        {
            Api = builder.Configuration.GetValue<string>($"{nameof(ExternalUrls)}:{nameof(ExternalUrls.Api)}") ?? string.Empty
        };

        builder.Services.AddSingleton(Options.Create(externalUrls));
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
                var url = sp.GetRequiredService<IOptions<ExternalUrls>>().Value.Api;
                client.BaseAddress = new Uri(url!);
            })
            .Services;
}