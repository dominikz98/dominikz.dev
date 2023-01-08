using System.Globalization;
using System.Threading.RateLimiting;
using dominikz.api.Models.Options;
using dominikz.api.Provider;
using dominikz.api.Provider.JustWatch;
using dominikz.api.Provider.Noobit;
using dominikz.api.Utils;
using dominikz.shared.Enums;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace dominikz.api.Extensions;

public static class DiExtensions
{
    public static IServiceCollection AddAuthPolicies(this IServiceCollection services)
        => services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CreateOrUpdate, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.CreateOrUpdate)));
            options.AddPolicy(Policies.Blog, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.Blog)));
            options.AddPolicy(Policies.Media, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.Media)));
            options.AddPolicy(Policies.Account, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.Account)));
        });

    private static bool HasPermission(AuthorizationHandlerContext context, PermissionFlags required)
    {
        if (context.User.TryGetFromClaim<PermissionFlags>(JwtHelper.ClaimPermissions, out var permissions) == false)
            return false;

        return permissions.HasFlag(required);
    }

    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)))
            .Configure<MedlanOptions>(builder.Configuration.GetSection(nameof(MedlanOptions)))
            .Configure<NoobitOptions>(builder.Configuration.GetSection(nameof(NoobitOptions)))
            .Configure<ImdbOptions>(builder.Configuration.GetSection(nameof(ImdbOptions)))
            .Configure<JustWatchOptions>(builder.Configuration.GetSection(nameof(JustWatchOptions)))
            .Configure<PasswordOptions>(builder.Configuration.GetSection(nameof(PasswordOptions)))
            .Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)))
            .Configure<ApiKeyOptions>(builder.Configuration.GetSection(nameof(ApiKeyOptions)));

        return builder;
    }

    public static WebApplicationBuilder AddClients(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<MedlanClient>()
            .AddScoped<NoobitClient>()
            .AddHttpClient<NoobitClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<NoobitOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
            })
            .Services.AddScoped<JustWatchClient>()
            .AddHttpClient<JustWatchClient>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<JustWatchOptions>>();
                client.BaseAddress = new Uri(options.Value.Url);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("JW DZ Client");
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

    public static WebApplicationBuilder AddRateLimit(this WebApplicationBuilder builder)
    {
        var options = new RateLimitOptions();
        builder.Configuration.GetSection(nameof(RateLimitOptions)).Bind(options);

        builder.Services.AddRateLimiter(limiter =>
        {
            limiter.OnRejected = (context, _) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return new ValueTask();
            };

            limiter.AddTokenBucketLimiter(Policies.RateLimit, conf =>
            {
                conf.TokenLimit = options.TokenLimit;
                conf.ReplenishmentPeriod = TimeSpan.FromSeconds(options.ReplenishmentPeriodInS);
                conf.TokensPerPeriod = options.TokensPerPeriod;
                conf.AutoReplenishment = options.AutoReplenishment;
            });
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
        builder.Services.AddScoped<CredentialsProvider>();
        builder.Services.AddScoped((services) =>
        {
            var accessor = services.GetRequiredService<IActionContextAccessor>();
            var factory = services.GetRequiredService<IUrlHelperFactory>();
            return factory.GetUrlHelper(accessor.ActionContext!);
        });

        // Add Services
        builder.Services.AddSingleton<PasswordHasher>()
            .AddSingleton<IStorageProvider, IoStorageProvider>()
            .AddScoped<ILinkCreator, LinkCreator>();

        //  Add Mediatr
        builder.Services.AddMediatR(typeof(Program));

        return builder;
    }
}