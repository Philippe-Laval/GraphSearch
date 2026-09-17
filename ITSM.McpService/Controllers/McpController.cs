using Microsoft.AspNetCore.Mvc;

// https://timdeschryver.dev/blog/your-first-mcp-server-with-aspnet

namespace ITSM.McpService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class McpController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ok", service = "ITSM.McpService" });
}
