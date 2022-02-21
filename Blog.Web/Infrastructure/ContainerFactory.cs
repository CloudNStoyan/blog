using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blog.Web.DAL;
using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Blog.Web.Infrastructure
{
    public static class ContainerFactory
    {
        public static IContainer Create(IConfiguration configuration, IServiceCollection serviceCollection = null)
        {
            var builder = new ContainerBuilder();
            
            if (serviceCollection != null)
            {
                builder.Populate(serviceCollection);
            }

            builder.RegisterModule(new MainModule(configuration));

            return builder.Build();
        }
    }

    public class MainModule : Autofac.Module
    {
        private IConfiguration Configuration { get; }

        public MainModule(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register((_, _) => new NpgsqlConnection(this.Configuration.GetValue<string>("DatabaseConnectionString"))).InstancePerLifetimeScope();

            builder.RegisterType<Database>().InstancePerLifetimeScope();

            builder.Register((_, _) => new MarkdownPipelineBuilder().UseAutoIdentifiers(AutoIdentifierOptions.GitHub).Build());

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
