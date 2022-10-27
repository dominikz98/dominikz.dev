using dominikz.api.Provider;
using dominikz.api.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace dominikz.api.Extensions;

public static class DIExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        // Add Context
        builder.Services.AddDbContext<DatabaseContext>(options =>
            //options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(DatabaseContext)))
            options.UseMySql(builder.Configuration.GetConnectionString(nameof(DatabaseContext)), MariaDbServerVersion.LatestSupportedServerVersion)
                .EnableDetailedErrors(builder.Environment.IsDevelopment())
                .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

        // Add Url-Helper
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
        builder.Services.AddScoped((services) =>
        {
            var accessor = services.GetRequiredService<IActionContextAccessor>();
            var factory = services.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(accessor.ActionContext!);
        });

        //  Add Provider
        builder.Services.AddScoped<ILinkCreator, LinkCreator>();
        builder.Services.AddScoped<IStorageProvider, StorageProvider>();
        builder.Services.AddScoped<NoobitClient>()
            .AddHttpClient<NoobitClient>((client) => client.BaseAddress = new Uri("https://www.noobit.dev/"));

        builder.Services.AddScoped<ImdbClient>()
            .AddHttpClient<ImdbClient>((client) => client.BaseAddress = new Uri("https://imdb-api.tprojects.workers.dev/"));

        //  Add Medatir
        builder.Services.AddMediatR(typeof(Program));

        return builder;
    }
}
