using System;
using System.Collections.Generic;
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

            Database database;
            using (var conn = new NpgsqlConnection(Service.ConnectionString))
            {
                database = new Database(conn);
            }

            builder.RegisterInstance(database);
            builder.RegisterType<Service>().As<IService>();

            return builder.Build();
        }
    }
}
