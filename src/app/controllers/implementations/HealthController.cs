using Microsoft.AspNetCore.Mvc;

namespace backend.app.implementations.Controllers
{
    [ApiController]
    [Route("health")]
    public sealed class HealthController : ControllerBase
    {
        public HealthController()
        {
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return Ok(new
            {
                message = "healthy",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("live")]
        public IActionResult Liveness()
        {
            return Ok(new
            {
                message = "healthy",
                timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new
            {
                message = "pong",
            });
        }
    }
}