[ApiController]
[Route("web-console")]
public class WebConsoleController(
    VmBookingRepository vmBookingRepository,
    UserSession session,
    VCenterApiService vCenterApiService
    ) : Controller
{
    [HttpGet("{vmUuid}")]
    public async Task<IActionResult> ShowWebConsole(string vmUuid)
    {
        var user = session.GetIfAuthenticated();
        var booking = await vmBookingRepository.GetByNameAsync(vmUuid);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        if (session.IsStudent() && booking.OwnerId != user.Id)
        {
            return Unauthorized(ResponseMessage.GetBookingNotFound());
        }

        var connectionUri = await vCenterApiService.GetVmConnectionUriAsync(vmUuid);
        if (connectionUri == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        if (connectionUri.Uri == null || connectionUri.Uri == "")
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        // get host ip from connectionUri
        string hostIp = connectionUri.Uri.Split("//")[1].Split(":")[0];

        var pageModel = new {
            booking.Name,
            connectionUri.Uri,
            HostIp = hostIp,
            booking.Login,
            booking.Password,
        };
        return View("web-console", pageModel);
    }
}