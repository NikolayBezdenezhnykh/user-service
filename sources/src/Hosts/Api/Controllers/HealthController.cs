using Microsoft.AspNetCore.Mvc;

namespace DockerBasics.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ApiVersionNeutral]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Check()
        {
            return Ok(new { Status = "OK" });
        }
    }
}
