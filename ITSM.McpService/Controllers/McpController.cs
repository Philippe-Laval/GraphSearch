using Microsoft.AspNetCore.Mvc;

namespace ITSM.McpService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class McpController : ControllerBase
{
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "ok", service = "ITSM.McpService" });
}
