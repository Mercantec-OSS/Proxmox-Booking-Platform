[ApiController]
[Route("web-console")]
public class WebConsoleController : Controller
{
    private readonly VmBookingService _vmBookingService;
    private readonly UserService _userService;
    private readonly UserSession _session;
    private readonly VCenterApiService _vCenterApiService;

    public WebConsoleController(Context context, UserSession session, VCenterApiService vCenterApiService)
    {
        _vmBookingService = new VmBookingService(context);
        _userService = new UserService(context);
        _session = session;
        _vCenterApiService = vCenterApiService;
    }



    [HttpGet("{vmUuid}")]
    public async Task<IActionResult> ShowWebConsole(string vmUuid)
    {
        var user = _session.GetIfAuthenticated();
        var booking = await _vmBookingService.GetByNameAsync(vmUuid);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        if (_session.IsStudent() && booking.OwnerId != user.Id)
        {
            return Unauthorized(ResponseMessage.GetBookingNotFound());
        }

        var connectionUri = await _vCenterApiService.GetVmConnectionUriAsync(vmUuid);
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