using System.Globalization;
using System.Threading.RateLimiting;
using dominikz.Api.Background;
using dominikz.Api.Utils;
using dominikz.Domain.Enums;
using dominikz.Domain.Options;
using dominikz.Infrastructure.Extensions;
using dominikz.Infrastructure.Provider;
using dominikz.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;

namespace dominikz.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAuthPolicies(this IServiceCollection services)
        => services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.CreateOrUpdate, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.CreateOrUpdate)));
                options.AddPolicy(Policies.Blog, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.Blog)));
                options.AddPolicy(Policies.Movies, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.Movies)));
                options.AddPolicy(Policies.Cookbook, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.Cookbook)));
                options.AddPolicy(Policies.Account, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.Account)));
                options.AddPolicy(Policies.Trades, policy => policy.RequireAssertion(context => HasPermission(context, PermissionFlags.Trades)));
            })
            .AddScoped<CredentialsProvider>();

    private static bool HasPermission(AuthorizationHandlerContext context, PermissionFlags required)
        => context.User.TryGetFromClaim<PermissionFlags>(JwtHelper.ClaimPermissions, out var permissions) && permissions.HasFlag(required);

    public static void AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)))
            .Configure<ExternalUrlsOptions>(builder.Configuration.GetSection(nameof(ExternalUrlsOptions)))
            .Configure<ApiKeysOptions>(builder.Configuration.GetSection(nameof(ApiKeysOptions)))
            .Configure<PasswordOptions>(builder.Configuration.GetSection(nameof(PasswordOptions)))
            .Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
    }

    public static void AddJwtAuth(this WebApplicationBuilder builder)
    {
        var options = new JwtOptions();
        builder.Configuration.GetSection(nameof(JwtOptions)).Bind(options);

        builder.Services.AddAuthentication(config =>
        {
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(config =>
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
    }

    public static void AddRateLimit(this WebApplicationBuilder builder)
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
    }

    public static void AddUrlHelper(this IServiceCollection services)
        => services.AddHttpContextAccessor()
            .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
            .AddScoped(sp =>
            {
                var accessor = sp.GetRequiredService<IActionContextAccessor>();
                var factory = sp.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(accessor.ActionContext!);
            });

    public static void AddWorker(this IServiceCollection services)
        => services.AddHostedService<PeriodicHostedService>()
            .AddSingleton<PeriodicHostedService>()
            .AddScoped<ITimeTriggeredWorker, FoodPriceSnapshotCreator>()
            .AddScoped<ITimeTriggeredWorker, ExternalArticleShadowMirror>()
            .AddScoped<ITimeTriggeredWorker, WhispersMirror>()
            .AddScoped<ITimeTriggeredWorker, FinnhubMirror>()
            .AddScoped<ITimeTriggeredWorker, TestService>();

    public static IServiceCollection AddUtils(this IServiceCollection services)
        => services.AddScoped<ILinkCreator, LinkCreator>()
            .AddSingleton<PasswordHasher>()
            .AddScoped<StreamTokenHandler>();
}