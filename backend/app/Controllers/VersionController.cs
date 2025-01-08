[ApiController]
[Route("version")]
public class VersionController : ControllerBase
{
    [HttpGet("/version")]
    public IActionResult GetVersion()
    {
        return Ok(Config.VERSION);
    }
}
