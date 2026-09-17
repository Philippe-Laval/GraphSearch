using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ITSM.McpService.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi(options =>
{
    // Specify the OpenAPI version to use
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
});

// MCP
builder.Services
    // Adds the Model Context Protocol (MCP) server to the service collection with default options.
    .AddMcpServer()
    // Adds the services necessary for McpEndpointRouteBuilderExtensions.MapMcp to handle MCP requests and sessions using the MCP Streamable HTTP transport.
    .WithHttpTransport()
    // Manually register your MCP tools.
    .WithTools<CatalogTools>();
    // Adds types marked with the ModelContextProtocol.Server.McpServerToolTypeAttribute attribute from the given assembly as tools to the server.
    //.WithToolsFromAssembly();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

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

app.MapMcp("mcp");
app.MapControllers();
app.MapDefaultEndpoints();

app.Run();
