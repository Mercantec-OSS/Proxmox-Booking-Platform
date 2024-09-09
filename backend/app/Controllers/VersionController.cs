[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    [HttpGet("/version")]
    public IActionResult GetVersion(Config config)
    {
        return Ok(config.VERSION);
    }
}
