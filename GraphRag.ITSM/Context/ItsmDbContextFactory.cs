using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRag.ITSM.Context
{
    public class ItsmDbContextFactory : IDesignTimeDbContextFactory<ItsmDbContext>
    {
        public ItsmDbContext CreateDbContext(string[] args)
        {
            //var connectionString = Environment.GetEnvironmentVariable("GRAPHRAG_CONNECTION_STRING")
            //    ?? "Server=(localdb)\\MSSQLLocalDB;Database=GraphRag;Trusted_Connection=True;TrustServerCertificate=True";

            // Cherche la configuration à côté de l'appli hôte (Console tool)
            // puis, à défaut, dans le répertoire courant.
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddUserSecrets<ItsmDbContext>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection introuvable pour la génération des migrations. " +
                    "Ajoutez-la dans appsettings.json / user-secrets / variable d'environnement.");

            var options = new DbContextOptionsBuilder<ItsmDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new ItsmDbContext(options);
        }
    }
}