var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ITSM_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

var mcpService = builder.AddProject<Projects.ITSM_McpService>("mcpservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.ITSM_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WithReference(mcpService)
    .WaitFor(apiService)
    .WaitFor(mcpService);

builder.Build().Run();
