using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
                     // İstek başına 5 dakikalık zaman aşımı
                     serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
                 })
                 .UseStartup<Startup>();
         });

    }
}
