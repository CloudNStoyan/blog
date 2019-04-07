using System;
using Autofac.Extensions.DependencyInjection;
using Blog.Web.Areas.Admin;
using Blog.Web.Areas.Admin.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Web
{
    public class Startup
    {
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();

            const string customName = "CustomAuthentication";

            services.AddAuthentication(customName)
                .AddCustomAuthentication(customName);

            services.AddMvc();

            services.AddHttpContextAccessor();

            return new AutofacServiceProvider(ContainerFactory.Create(services));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
