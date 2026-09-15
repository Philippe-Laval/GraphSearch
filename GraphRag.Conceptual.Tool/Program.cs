using GraphRag.Conceptual.Context;
using GraphRag.Conceptual.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace GraphRag.Conceptual.Tool
{
    internal class Program
    {
        public static async Task<int> Main(string[] args)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource.Token;

            Log.Logger = new LoggerConfiguration()
                            .WriteTo.Console()
                            .CreateBootstrapLogger();

            try
            {
                HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

                builder.Services.AddSerilog((services, configuration) => configuration
                    .ReadFrom.Configuration(builder.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext());

                // Reads ConnectionStrings:DefaultConnection from configuration (throws with a clear message if missing).
                var connectionString =
                    builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException(
                        "ConnectionStrings:DefaultConnection is missing. " +
                        "Add it to appsettings.json / user-secrets / environment variables.");

                // Registers ItsmDbContext with AddDbContext<ItsmDbContext> using SQL Server,
                // and points MigrationsAssembly at the GraphRag.ITSM assembly.
                builder.Services.AddDbContext<OntologyDbContext>(options =>
                    options.UseSqlServer(connectionString, sql =>
                        sql.MigrationsAssembly(typeof(OntologyDbContext).Assembly.FullName)));

                // Register all CRUD services to access OntologyDbContext
                builder.Services.AddConceptualServices();

                builder.Services.AddScoped<IConceptualDbSeeder, ConceptualDbSeeder>();

                using IHost host = builder.Build();

                // Seed the database
                using (var scope = host.Services.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<IConceptualDbSeeder>();
                    await seeder.Seed(cancellationToken);
                }

                await host.RunAsync();

                return 0;
            }
            catch (OptionsValidationException exception)
            {
                Log.Fatal(exception, "Invalid application configuration");
                return 2;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Application terminated unexpectedly");
                return 1;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }

        }
    }
}
