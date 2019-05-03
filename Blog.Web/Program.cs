using Blog.Web.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Blog.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(x => x.AddServerHeader = false);
        }
    }
}
