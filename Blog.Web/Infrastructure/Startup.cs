using System;
using Autofac.Extensions.DependencyInjection;
using Blog.Web.Areas.Admin.Auth;
using Blog.Web.ErrorHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Web.Infrastructure
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

            app.UseNotFoundMiddleware();

            app.UseStaticFiles();

            app.UseAuthMiddleware();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "create",
                    template: "{area:exists}/{controller}/create",
                    defaults: new { controller = "Posts", action = "CreateOrEdit" }
                );

                routes.MapRoute(
                    name: "edit",
                    template: "{area:exists}/{controller}/edit/{id}",
                    defaults: new { controller = "Posts", action = "CreateOrEdit" }
                );

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
