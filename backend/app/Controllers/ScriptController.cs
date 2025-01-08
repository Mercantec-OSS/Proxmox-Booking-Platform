[Route("script")]
[ApiController]
public class ScriptController(
    VCenterApiService vCenterApiService,
    UserSession session, 
    VmBookingService vmBookingService, 
    VmBookingScriptService vmBookingScriptService
    ) : ControllerBase
{
    [HttpGet("vm/get-ip/{name}")]
    public async Task<ActionResult> GetVmIp(string name)
    {
        session.IsAuthenticated();

        VmBooking? booking = await vmBookingService.GetByNameAsync(name);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        VmInfoGetDto? vmInfo = await vCenterApiService.GetInfo(name);
        if (vmInfo == null)
        {
            return NotFound(ResponseMessage.GetErrorMessage("Error under converting vm info"));
        }

        vmInfo.Username = booking.Login;
        vmInfo.Password = booking.Password;
        
        return Ok(vmInfo);
    }
    [HttpGet("vm/reset-power/{name}")]
    public async Task<ActionResult> ResetVmPower(string name)
    {
        session.IsAuthenticated();

        VmBooking? booking = await vmBookingService.GetByNameAsync(name);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }
        vmBookingScriptService.ResetPower(booking.Name);
        return NoContent();
    }

    [HttpGet("vm/templates")]
    public ActionResult GetTemplates()
    {
        session.IsAuthenticated();
        return Ok(TemplateGetDto.MakeGetDtoFromList(TemplatesBackgroundService.GetTemplates()));
    }

    [HttpDelete("vm/reset-templates")]
    public ActionResult ResetTemplates()
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        TemplatesBackgroundService.ResetTemplates();
        return NoContent();
    }

    [HttpGet("vm/vcenter-info")]
    public ActionResult GetVcenterInfo()
    {
        return Ok(VCenterInfoBackgroundService.GetInfo());
    }
}
