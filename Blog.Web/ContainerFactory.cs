using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blog.Web.Areas.Admin.Services;
using Blog.Web.DAL;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Blog.Web
{
    public static class ContainerFactory
    {
        public static IContainer Create(IServiceCollection serviceCollection = null)
        {
            var builder = new ContainerBuilder();
            
            if (serviceCollection != null)
            {
                builder.Populate(serviceCollection);
            }
            
            builder.Register((ctx, p) => new NpgsqlConnection("Server=vm13.lan;Port=4401;Database=blog;Uid=blog;Pwd=test123;")).InstancePerLifetimeScope();
      
            builder.RegisterType<PostService>().InstancePerLifetimeScope();
            builder.RegisterType<Database>().InstancePerLifetimeScope();
            builder.RegisterType<AuthenticationService>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
