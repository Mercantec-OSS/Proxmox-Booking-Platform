[ApiController]
[Route("web-console")]
public class WebConsoleController : Controller
{
    private readonly VmBookingService _vmBookingService;
    private readonly UserService _userService;
    private readonly EmailService _emailService;
    private readonly UserSession _session;
    private readonly ScriptService _scriptService;

    public WebConsoleController(Context context, Config config, UserSession session, ScriptService scriptService)
    {
        _vmBookingService = new VmBookingService(context);
        _userService = new UserService(context);
        _emailService = new EmailService(config);
        _session = session;
        _scriptService = scriptService;
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

        var connectionUri = await _scriptService.GetVmConnectionUriAsync(vmUuid);
        if (connectionUri == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        if (connectionUri.Uri == null || connectionUri.Uri == "")
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }
        
        var pageModel = new {
            url = connectionUri.Uri,
            vcenterIp = connectionUri.VcenterIp,
            booking.Login,
            booking.Password,
        };
        return View("web-console", pageModel);
    }
}