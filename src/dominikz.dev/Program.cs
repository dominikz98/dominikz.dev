using dominikz.dev;
using dominikz.dev.Components.Toast;
using dominikz.dev.Endpoints;
using dominikz.dev.Models;
using dominikz.dev.Utils;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.AddOptions();

builder.Services.AddScoped<BrowserService>()
    .AddScoped<ToastService>()
    .AddScoped<BlogEndpoints>()
    .AddScoped<CookbookEndpoints>()
    .AddScoped<MediaEndpoints>()
    .AddScoped<MovieEndpoints>()
    .AddScoped<GameEndpoints>()
    .AddScoped<BookEndpoints>()
    .AddScoped<MusicEndpoints>()
    .AddHttpClient<ApiClient>((sp, client) =>
    {
        var url = sp.GetRequiredService<IOptions<ExternalUrls>>().Value.Api;
        client.BaseAddress = new Uri(url!);
    });

await builder.Build().RunAsync();