using dominikz.kernel.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace dominikz.kernel;

public static class Extensions
{
    public static IServiceCollection AddApiEndpoints(this IServiceCollection services)
        => services.AddScoped<BlogEndpoints>()
            .AddScoped<MediasEndpoints>();
}
