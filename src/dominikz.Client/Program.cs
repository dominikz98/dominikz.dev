using Blazored.LocalStorage;
using dominikz.Client;
using dominikz.Infrastructure;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.AddOptions();

builder.Services.AddBlazoredLocalStorageAsSingleton()
    .AddApiClient(builder.HostEnvironment.BaseAddress)
    .AddServices();

await builder.Build().RunAsync();