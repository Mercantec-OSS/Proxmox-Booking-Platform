[ApiController]
[Route("version")]
public class VersionController(ProxmoxApiService proxmoxApiService) : ControllerBase
{
    [HttpGet("/version")]
    public async Task<IActionResult> GetVersion()
    {
        return Ok(await proxmoxApiService.GetTemplates());
    }
}
