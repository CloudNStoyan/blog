using System;
using Autofac.Extensions.DependencyInjection;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.ErrorHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Blog.Web.Infrastructure
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();

            const string customName = "CustomAuthentication";

            services.AddAuthentication(customName)
                .AddCustomAuthentication(customName);

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            });

            services.AddHttpContextAccessor();

            return new AutofacServiceProvider(ContainerFactory.Create(this.Configuration, services));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseNotFoundMiddleware();

            app.UseStaticFiles();

            app.UseAuthMiddleware();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
