[Route("script")]
[ApiController]
public class ScriptController(
    ScriptService scriptService, 
    UserSession session, 
    ClusterBookingService clusterBookingService, 
    VmBookingService vmBookingService, 
    VCenterService vCenterService,
    EsxiHostService esxiHostService
    ) : ControllerBase
{
    [HttpGet("cluster/host/reset-by-booking-id/{id}")]
    public async Task<ActionResult> ResetHosts(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        ClusterBooking? booking = await clusterBookingService.GetByIdAsync(id); 

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        return Ok(await scriptService.ResetClusterBookingAsync(booking.VCenters));
    }

    [HttpGet("cluster/vcenter/install-by-booking-id/{id}")]
    public async Task<ActionResult> InstallVcenters(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        ClusterBooking? booking = await clusterBookingService.GetByIdAsync(id); 

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        return Ok(await scriptService.CreateClusterBooking(booking.VCenters));
    }

    [HttpGet("cluster/vcenter/reset-and-install-by-booking-id/{id}")]
    public async Task<ActionResult> ResetAndInstallVcenters(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        ClusterBooking? booking = await clusterBookingService.GetByIdAsync(id); 

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        return Ok(await scriptService.ResetAndInstallVcenterAsync(booking.VCenters));
    }

    [HttpGet("cluster/vcenter/install-by-vcenter-id/{id}")]
    public async Task<ActionResult> InstallVcenter(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        VCenter? vcenter = await vCenterService.GetByIdAsync(id);

        if (vcenter == null)
        {
            return NotFound(ResponseMessage.GetVCenterNotFound());
        }

        return Ok(await scriptService.InstallVCenterAsync(vcenter.JsonConfig));
    }

    [HttpGet("cluster/vcenter/reset-and-install-by-vcenter-id/{id}")]
    public async Task<ActionResult> ResetAndInstallVcenter(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        VCenter? vcenter = await vCenterService.GetByIdAsync(id);

        if (vcenter == null)
        {
            return NotFound(ResponseMessage.GetVCenterNotFound());
        }

        List<VCenter> vcenters = new List<VCenter> { vcenter };
        return Ok(await scriptService.ResetAndInstallVcenterAsync(vcenters));
    }

    [HttpGet("cluster/host/reset-by-id/{id}")]
    public async Task<ActionResult> ResetEsxiHost(int id)
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        EsxiHost? host = await esxiHostService.GetByIdAsync(id);

        if (host == null)
        {
            return NotFound(ResponseMessage.GetEsxiHostNotFound());
        }

        return Ok(await scriptService.ResetHostAsync(host));
    }

    [HttpGet("vm/get-ip/{name}")]
    public async Task<ActionResult> GetVmIp(string name)
    {
        session.IsAuthenticated();

        VmBooking? booking = await vmBookingService.GetByNameAsync(name);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        VmInfoGetDto? vmInfo = await scriptService.GetVmAsync(booking.Name);
        if (vmInfo == null)
        {
            return NotFound(ResponseMessage.GetErrorMessage("Error under converting vm info"));
        }

        vmInfo.username = booking.Login;
        vmInfo.password = booking.Password;
        
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
        await scriptService.ResetPowerVmAsync(booking.Name);
        return NoContent();
    }

    [HttpGet("vm/templates")]
    public async Task<ActionResult> GetTemplates()
    {
        session.IsAuthenticated();
        return Ok(await scriptService.GetTemplatesAsync());
    }

    [HttpDelete("vm/reset-templates")]
    public async Task<ActionResult> ResetTemplates()
    {
        session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher
        );

        await scriptService.ResetTemplatesAsync();
        return NoContent();
    }

    [HttpGet("vm/vcenter-info")]
    public ActionResult GetVcenterInfo()
    {
        return Ok(VCenterInfoBackgroundService.GetInfo());
    }
}
