[ApiController]
[Route("web-console")]
public class WebConsoleController(
    VmBookingRepository vmBookingRepository,
    UserSession session,
    ProxmoxApiService proxmoxApiService,
    WebsockifyService websockifyService
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

        var vm = await proxmoxApiService.GetVmByNameAsync(vmUuid);
        if (vm == null) {
            return NotFound(ResponseMessage.GetErrorMessage("VM not found"));
        }

        // get vnc info
        var vncInfo = await proxmoxApiService.GetVncInfo(vm);

        // start websockify process
        int websockifyPort = websockifyService.Start(vncInfo);

        // await for process to start
        await Task.Delay(1000);

        var pageModel = new {
            booking.Name,
            VmLogin = booking.Login,
            VmPassword = booking.Password,
            VncPort = websockifyPort,
            VncPassword = WebUtility.UrlEncode(vncInfo.Password),
        };

        return View("web-console", pageModel);
    }

    [HttpGet("novnc")]
    public IActionResult ShowNoVNCConsole() {
        return View("novnc");
    }
}