using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using dominikz.Client;
using dominikz.Client.Extensions;
using dominikz.Infrastructure;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddApiClient()
    .AddApiOptions(builder.Configuration)
    .AddServices()
    .AddBlazoredLocalStorageAsSingleton();

await builder.Build().RunAsync();