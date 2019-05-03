using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blog.Web.DAL;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Blog.Web.Infrastructure
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

            builder.RegisterModule<MainModule>();
           
            return builder.Build();
        }
    }

    public class MainModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((ctx, p) => new NpgsqlConnection("Server=vm13.lan;Port=4401;Database=blog;Uid=blog;Pwd=test123;")).InstancePerLifetimeScope();

            builder.RegisterType<Database>().InstancePerLifetimeScope();

            var serviceTypes = Assembly.GetExecutingAssembly()
                .DefinedTypes.Where(x => x.IsClass && x.Name.EndsWith("Service")).ToList();

            foreach (var serviceType in serviceTypes)
            {
                builder.RegisterType(serviceType).InstancePerLifetimeScope();
            }

            base.Load(builder);
        }
    }
}
