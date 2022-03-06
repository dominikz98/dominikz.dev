using dominikz.Components.Models;
using dominikz.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace dominikz
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped<DataContext>();
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5001/api") });

            builder.Services.AddScoped<CodeSyntaxStyleSheet>();
            builder.Services.AddScoped((_) =>
               new CSSTheme()
               {
                   Primary = "#FFD369",
                   Secondary = "#EEEEEE",
                   Surface = "#25262C",
                   Background = "#191A1F",
                   Menu = "#0F0F10"
               });
            builder.Services.AddScoped<CSSBuilder>();

            await builder.Build().RunAsync();
        }
    }
}
