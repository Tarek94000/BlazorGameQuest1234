using Microsoft.AspNetCore.Mvc;

namespace AuthenticationServices.Controllers;

[ApiController]
[Route("[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("API OK - BlazorGameQuest");
}
