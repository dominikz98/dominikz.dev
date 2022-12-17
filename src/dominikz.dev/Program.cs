using dominikz.dev;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<BrowserService>()
    .AddScoped<BlogEndpoints>()
    .AddScoped<CookbookEndpoints>()
    .AddScoped<MediaEndpoints>()
    .AddScoped<MovieEndpoints>()
    .AddScoped<GameEndpoints>()
    .AddScoped<BookEndpoints>()
    .AddHttpClient<ApiClient>((sp, client) =>
    {
        var url = builder.Configuration.GetValue<string>($"{nameof(ExternalUrls)}:{nameof(ExternalUrls.Api)}");
        client.BaseAddress = new Uri(url!);
    });

await builder.Build().RunAsync();