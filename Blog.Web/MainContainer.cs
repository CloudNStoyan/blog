using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Blog.Web.DAL;
using Npgsql;

namespace Blog.Web
{
    public static class MainContainer
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            
            builder.Register((ctx, p) => new NpgsqlConnection(Service.ConnectionString)).InstancePerLifetimeScope();
      
            builder.RegisterType<Service>().InstancePerLifetimeScope();
            builder.RegisterType<Database>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
