using dominikz.api.Models.Options;
using dominikz.api.Provider;
using dominikz.api.Utils;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace dominikz.api.Extensions;

public static class DiExtensions
{
    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)))
            .Configure<MedlanOptions>(builder.Configuration.GetSection(nameof(MedlanOptions)))
            .Configure<NoobitOptions>(builder.Configuration.GetSection(nameof(NoobitOptions)))
            .Configure<ImdbOptions>(builder.Configuration.GetSection(nameof(ImdbOptions)))
            .Configure<PasswordOptions>(builder.Configuration.GetSection(nameof(PasswordOptions)))
            .Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));

        return builder;
    }

    public static WebApplicationBuilder AddClients(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IStorageProvider, StorageProvider>()
            .AddScoped<MedlanClient>()
            .AddScoped<NoobitClient>()
            .AddHttpClient<NoobitClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<NoobitOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
            })
            .Services
            .AddScoped<ImdbClient>()
            .AddHttpClient<ImdbClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<ImdbOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
            });

        return builder;
    }

    public static WebApplicationBuilder AddJwtAuth(this WebApplicationBuilder builder)
    {
        var options = new JwtOptions();
        builder.Configuration.GetSection(nameof(JwtOptions)).Bind(options);
        
        builder.Services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer((config) =>
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(options.IssuerSigningKey)),
                ValidateIssuer = options.ValidateIssuer,
                ValidIssuer = options.ValidIssuer,
                ValidateAudience = options.ValidateAudience,
                ValidAudience = options.ValidAudience,
                RequireExpirationTime = options.RequireExpirationTime,
                ValidateLifetime = options.ValidateLifetime,
                ClockSkew = TimeSpan.FromHours(options.LifetimeInH),
            };
        });

        return builder;
    }

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

        // Add Services
        builder.Services.AddSingleton<PasswordHasher>()
            .AddScoped<ILinkCreator, LinkCreator>();

        //  Add Mediatr
        builder.Services.AddMediatR(typeof(Program));

        return builder;
    }
}