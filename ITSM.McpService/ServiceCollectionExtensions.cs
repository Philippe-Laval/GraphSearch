using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ModelContextProtocol.AspNetCore.Authentication;
using ModelContextProtocol.Authentication;

namespace ITSM.McpService
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureMcpSecurity
        (
            this IServiceCollection serviceCollection,
            IConfiguration configuration
        )
        {
            serviceCollection
              .AddAuthentication
              (
                options =>
                {
                    options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                }
              )
              .AddJwtBearer
              (
                "McpBearer",
                options =>
                {
                    options.Authority = configuration["AzureAd:Authority"];
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = configuration["McpServer:AzureAd:ClientId"],
                        ValidIssuer = configuration["AzureAd:Issuer"],
                        NameClaimType = "name",
                        RoleClaimType = "roles",
                    };
                }
              )
              .AddMcp
              (
                options =>
                {
                    options.ResourceMetadata = new ProtectedResourceMetadata
                    {
                        Resource = configuration["McpServer:Url"],
                        ResourceDocumentation = "https://localhost:<port>",
                        AuthorizationServers = { configuration["AzureAd:Issuer"]! },
                        ScopesSupported = [$"api://{configuration["McpServer:AzureAd:ClientId"]}"],
                    };
                }
              );

            return serviceCollection;
        }
    }
}
