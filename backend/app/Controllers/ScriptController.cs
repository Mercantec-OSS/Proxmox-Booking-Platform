[Route("script")]
[ApiController]
public class ScriptController(
    VCenterApiService vCenterApiService,
    UserSession session, 
    VmBookingRepository vmBookingRepository,
    VmBookingScriptService vmBookingScriptService
    ) : ControllerBase
{
    [HttpGet("vm/get-ip/{name}")]
    public async Task<ActionResult> GetVmIp(string name)
    {
        User user = session.GetIfAuthenticated();
        VmBooking? booking = await vmBookingRepository.GetByNameAsync(name);

        if (booking == null)
        {
            return Ok(new VmInfoGetDto());
        }

        // Deny access to the booking if the user is a student and the booking is not his
        if (session.IsStudent() && booking.OwnerId != user.Id)
        {
            return NotFound(ResponseMessage.GetUserUnauthorized());
        }

        // Deny access to the booking if the user is a teacher and the booking is not his
        if (session.IsTeacher() && booking.AssignedId != user.Id)
        {
            return NotFound(ResponseMessage.GetUserUnauthorized());
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

        VmBooking? booking = await vmBookingRepository.GetByNameAsync(name);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }
        vmBookingScriptService.ResetPower(booking.Name);
        return NoContent();
    }

    [HttpPut("vm/update-resources")]
    public async Task<ActionResult> UpdateVmResources(VmUpdateResourcesDto dto)
    {
        var user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        VmBooking? booking = await vmBookingRepository.GetByNameAsync(dto.Uuid);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        // Deny access to the booking if the user is a teacher and the booking is not his
        if (user.IsTeacher() && booking.OwnerId != user.Id)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        vmBookingScriptService.Update(booking.Name, dto.Cpu, dto.Ram);
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
