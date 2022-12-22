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
}