using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GraphRag.EFCore.Context;

public class GraphRagDbContextFactory : IDesignTimeDbContextFactory<GraphRagDbContext>
{
    public GraphRagDbContext CreateDbContext(string[] args)
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
            .AddUserSecrets<GraphRagDbContext>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:DefaultConnection introuvable pour la génération des migrations. " +
                "Ajoutez-la dans appsettings.json / user-secrets / variable d'environnement.");

        var options = new DbContextOptionsBuilder<GraphRagDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new GraphRagDbContext(options);
    }
}
