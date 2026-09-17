using System.Text.Json.Serialization;
using GraphRag.ITSM.Context;
using GraphRag.ITSM.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Get the connection string from configuration (appsettings.json, user-secrets, or environment variables)
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection is missing. Add it to appsettings.json, user-secrets, or environment variables.");

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi(options =>
{
    // Specify the OpenAPI version to use
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
});

// Register the ITSM DbContext with SQL Server provider and specify the migrations assembly
builder.Services.AddDbContext<ItsmDbContext>(options =>
    options.UseSqlServer(connectionString, sql =>
        sql.MigrationsAssembly(typeof(ItsmDbContext).Assembly.FullName)));

// Register ITSM services
builder.Services.AddItsmServices();

// Register controllers and configure JSON serialization options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/overview?view=aspnetcore-10.0
    // Enable OpenAPI/Swagger in development
    // Launch the app and navigate to http://localhost:5385/openapi/v1.json to view the generated OpenAPI document
    app.MapOpenApi();

    // For custom OpenAPI document paths, you can use the following mappings:
    //app.MapOpenApi("/openapi/{documentName}.yaml");

    // Use Swagger UI for local ad-hoc testing
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/using-openapi-documents?view=aspnetcore-10.0
    // Launch the app and navigate to https://localhost:5385/swagger to view the Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
