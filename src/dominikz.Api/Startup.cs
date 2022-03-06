using dominikz.Api.Commands;
using dominikz.Api.Models.Options;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;

namespace dominikz.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(policy =>
            {
                policy.AddPolicy("CorsPolicy", opt => opt
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            services.Configure<PodcastOptions>(Configuration.GetSection(nameof(PodcastOptions)));

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped(factory =>
            {
                var context = factory.GetRequiredService<IActionContextAccessor>().ActionContext;
                var urlFactory = factory.GetRequiredService<IUrlHelperFactory>();
                return urlFactory.GetUrlHelper(context);
            });

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite($"Data Source={Configuration.GetConnectionString("Database")}");
            });

            services.AddHttpClient<GetPodcastHandler>("noobit", (sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<PodcastOptions>>().Value;
                client.BaseAddress = new Uri(options.NoobitUrl);
            });

            services.AddMediatR(typeof(Startup));
            services.AddAutoMapper(typeof(Startup));
            services.AddImageSharp();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            app.UseImageSharp();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
