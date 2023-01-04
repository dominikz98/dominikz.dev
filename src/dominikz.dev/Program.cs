using Blazored.LocalStorage;
using dominikz.dev;
using dominikz.dev.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.AddOptions();

builder.Services.AddBlazoredLocalStorageAsSingleton()
    .AddServices()
    .AddEndpoints();

await builder.Build().RunAsync();