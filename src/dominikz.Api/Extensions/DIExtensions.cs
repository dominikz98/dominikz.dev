using dominikz.Api.Provider;
using dominikz.Api.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

namespace dominikz.Api.Extensions;

public static class DIExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
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

        //  Add Medatir
        builder.Services.AddMediatR(typeof(Program));

        return builder;
    }
}
