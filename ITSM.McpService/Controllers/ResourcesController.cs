using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Net.Mime;

// https://octelys.medium.com/setting-up-an-mcp-server-with-oauth-authentication-in-asp-net-core-a-complete-guide-135a59659e75

// Show UI in Azure AD to consent to the application and its permissions.
// The consent screen will display the name of the application, the permissions it is requesting,
// and any additional information provided by the developer.
// The user can then choose to grant or deny consent.
// https://blog.mitchbarry.com/net-mcp-server-oauth-with-microsoft-entra-id/

namespace ITSM.McpService.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    // Marks the controller as MCP-enabled
    [McpServerToolType]
    public sealed class ResourcesController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ResourceContract>), StatusCodes.Status200OK)]
        //  Exposes the method as an MCP tool with a specific name
        [McpServerTool(Name = "GetResources")]
        // Provides a description for the MCP tool
        [Description("Super duper description that the agent can understand")]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken = default)
        {
            // Implementation here
            return Ok(new List<ResourceContract>
            {
                new ResourceContract
                (   Guid.NewGuid(),
                    "Resource 1",
                    "Description for Resource 1"
                ),
                new ResourceContract
                (
                    Guid.NewGuid(),
                    "Resource 2",
                    "Description for Resource 2"
                )
            });
        }
    }

    public record ResourceContract(Guid Id, string Name, string Description);
    
}
