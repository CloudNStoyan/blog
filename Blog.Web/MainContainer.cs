using Autofac;
using Blog.Web.Areas.Admin.Services;
using Blog.Web.DAL;
using Npgsql;

namespace Blog.Web
{
    public static class MainContainer
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();
            
            builder.Register((ctx, p) => new NpgsqlConnection("Server=vm13.lan;Port=4401;Database=blog;Uid=blog;Pwd=test123;")).InstancePerLifetimeScope();
      
            builder.RegisterType<PostService>().InstancePerLifetimeScope();
            builder.RegisterType<Database>().InstancePerLifetimeScope();
            builder.RegisterType<AuthenticationService>().InstancePerLifetimeScope();

            return builder.Build();
        }
    }
}
