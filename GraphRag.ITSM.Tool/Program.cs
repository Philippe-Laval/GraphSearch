using GraphRag.ITSM.Context;
using GraphRag.ITSM.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace GraphRag.ITSM.Tool
{
    public class Program
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
                builder.Services.AddDbContext<ItsmDbContext>(options =>
                    options.UseSqlServer(connectionString, sql =>
                        sql.MigrationsAssembly(typeof(ItsmDbContext).Assembly.FullName)));

                // Register all CRUD services to access ItsmDbContext
                builder.Services.AddItsmServices();

                builder.Services.AddScoped<IItsmDbSeeder, ItsmDbSeeder>();

                using IHost host = builder.Build();

                // Seed the database
                using (var scope = host.Services.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<IItsmDbSeeder>();
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
