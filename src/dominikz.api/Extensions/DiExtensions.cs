using dominikz.api.Models.Options;
using dominikz.api.Provider;
using dominikz.api.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace dominikz.api.Extensions;

public static class DiExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        // Add Options
        builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)));
        builder.Services.Configure<MedlanOptions>(builder.Configuration.GetSection(nameof(MedlanOptions)));
        builder.Services.Configure<NoobitOptions>(builder.Configuration.GetSection(nameof(NoobitOptions)));
        builder.Services.Configure<ImdbOptions>(builder.Configuration.GetSection(nameof(ImdbOptions)));
        builder.Services.Configure<DominikZOptions>(builder.Configuration.GetSection(nameof(DominikZOptions)));
        
        // Add Context
        builder.Services.AddDbContext<DatabaseContext>(options =>
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
        builder.Services.AddScoped<MedlanClient>();
        builder.Services.AddScoped<NoobitClient>()
            .AddHttpClient<NoobitClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<NoobitOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
            });

        builder.Services.AddScoped<ImdbClient>()
            .AddHttpClient<ImdbClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ImdbOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
            });

        //  Add Mediatr
        builder.Services.AddMediatR(typeof(Program));

        return builder;
    }
}
