using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace realEstateManagement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(serverOptions =>
                    {
                        serverOptions.Listen(IPAddress.Any, 80); // HTTP port
                        serverOptions.Listen(IPAddress.Any, 443, listenOptions =>
                        {
                            listenOptions.UseHttps("mycert.pfx", "23323847lol"); // HTTPS port
                        });
                    })
                    .UseStartup<Startup>();
                });
    }
}
