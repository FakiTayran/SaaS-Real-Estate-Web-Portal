using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace realEstateManagementDataLayer.EntityFramework
{
    public class RealEstateManagementDbContextFactory : IDesignTimeDbContextFactory<RealEstateManagementDbContext>
    {
        public RealEstateManagementDbContext CreateDbContext(string[] args)
        {
            // appsettings.json dosyasının yolunu belirleyin
            var basePath = Directory.GetCurrentDirectory() + string.Format("{0}..{0}realEstateManagementAPI", Path.DirectorySeparatorChar);
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Yapılandırma dosyasını yükleyin
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", true)
                .Build();

            // Bağlantı dizesini alın
            var connectionString = configuration.GetConnectionString("Default");

            // DbContextOptionsBuilder'ı yapılandırın
            var builder = new DbContextOptionsBuilder<RealEstateManagementDbContext>();
            builder.UseNpgsql(connectionString);

            // Yapılandırılmış DbContext'i döndürün
            return new RealEstateManagementDbContext(builder.Options);
        }
    }
}
