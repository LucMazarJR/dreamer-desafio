using Microsoft.AspNetCore.Mvc;

namespace dedg_back.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TimeController : ControllerBase
{
    [HttpGet]
    public IActionResult GetServerTime()
    {
        return Ok(new { utc = DateTime.UtcNow });
    }
}
