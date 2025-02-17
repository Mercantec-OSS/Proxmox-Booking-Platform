﻿[Route("script")]
[ApiController]
public class ScriptController(
    UserSession session, 
    VmBookingRepository vmBookingRepository,
    ProxmoxApiService proxmoxApiService,
    VmService vmService
    ) : ControllerBase
{
    [HttpGet("vm/get-ip/{name}")]
    public async Task<ActionResult> GetVmIp(string name)
    {
        User user = session.GetIfAuthenticated();
        VmBooking? booking = await vmBookingRepository.GetByNameAsync(name);

        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
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

        ProxmoxVmDto? vm = await vmService.GetVmByNameAsync(booking.Name);
        if (vm == null)
        {
            return NotFound(ResponseMessage.GetErrorMessage("Vm not found"));
        }

        // search internal ip (starts with 10.x.x.x)
        List<ProxmoxNetworkDeviceDto> devices = await proxmoxApiService.GetVmNetworkDevices(vm);
        string ip = "";
        foreach (var device in devices)
        {
            foreach (var ipAddr in device.IpAddresses)
            {
                if (ipAddr.IsIpv4 && ipAddr.IpAddress.StartsWith("10."))
                {
                    ip = ipAddr.IpAddress;
                    break;
                }
            }
        }

        VmInfoGetDto? vmInfo = new(){
            Ip = ip,
            Name = vm.Name,
            Username = booking.Login,
            Password = booking.Password,
            Cpu = vm.MaxCPU,
            Ram = vm.MaxRamGb
        };

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

        _ = vmService.ResetPower(booking.Name);
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

        // Deny access to the booking if the user is a teacher and it not assigned to booking
        if (user.IsTeacher() && booking.AssignedId != user.Id)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        // Make limits for resources
        if (dto.Cpu < 1 || dto.Cpu > 6)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("Cpu must be between 1 and 6"));
        }

        if (dto.Ram < 2 || dto.Ram > 16)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("Ram must be between 2 and 16"));
        }

        _ = vmService.UpdateVmResources(booking.Name, dto.Cpu, dto.Ram * 1024);
        return NoContent();
    }

    [HttpGet("vm/templates")]
    public async Task<ActionResult> GetTemplates()
    {
        session.IsAuthenticated();
        return Ok(await proxmoxApiService.GetTemplates());
    }

    [HttpGet("vm/iso-list")]
    public async Task<ActionResult> GetIsoList()
    {
        session.IsAuthenticated();
        List<IsoDto> isoList = await vmService.GetIsoList();
        return Ok(isoList);
    }

    [HttpPost("vm/attach-iso")]
    public async Task<ActionResult> AttachIso([FromBody] IsoAttachDto dto)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Student
        );

        VmBooking? booking = await vmBookingRepository.GetByNameAsync(dto.VmName);
        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        // Deny access to the booking if the user is a student and the booking is not his
        if (user.IsStudent() && booking.OwnerId != user.Id)
        {
            return NotFound(ResponseMessage.GetUserUnauthorized());
        }

        // Deny access to the booking if the user is a teacher and the booking not assigned to him
        if (user.IsTeacher() && booking.AssignedId != user.Id)
        {
            return NotFound(ResponseMessage.GetUserUnauthorized());
        }

        await vmService.AttachIso(booking.Name, dto.IsoName);
        return NoContent();
    }

    [HttpPost("vm/detach-iso")]
    public async Task<ActionResult> DetachIso([FromBody] IsoDetachDto dto)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Student
        );

        VmBooking? booking = await vmBookingRepository.GetByNameAsync(dto.VmName);
        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        // Deny access to the booking if the user is a student and the booking is not his
        if (user.IsStudent() && booking.OwnerId != user.Id)
        {
            return NotFound(ResponseMessage.GetUserUnauthorized());
        }

        // Deny access to the booking if the user is a teacher and the booking not assigned to him
        if (user.IsTeacher() && booking.AssignedId != user.Id)
        {
            return NotFound(ResponseMessage.GetUserUnauthorized());
        }

        await vmService.DetachIso(booking.Name);
        return NoContent();
    }

    [HttpPost("vm/attach-storage")]
    public async Task<ActionResult> AttachStorage([FromBody] StorageAttachDto dto)
    {
        User user = session.GetIfRoles
        (
            Models.User.UserRoles.Admin,
            Models.User.UserRoles.Teacher,
            Models.User.UserRoles.Student
        );

        VmBooking? booking = await vmBookingRepository.GetByNameAsync(dto.VmName);
        if (booking == null)
        {
            return NotFound(ResponseMessage.GetBookingNotFound());
        }

        // Deny access to the booking if the user is a student and the booking is not his
        if (user.IsStudent() && booking.OwnerId != user.Id)
        {
            return NotFound(ResponseMessage.GetUserUnauthorized());
        }

        // Deny access to the booking if the user is a teacher and the booking not assigned to him
        if (user.IsTeacher() && booking.AssignedId != user.Id)
        {
            return NotFound(ResponseMessage.GetUserUnauthorized());
        }

        // Deny access to the booking if amount of storage more than 500
        if (dto.AmountGb > 500)
        {
            return BadRequest(ResponseMessage.GetErrorMessage("Amount of storage more than 500"));
        }

        await vmService.AddStorage(booking.Name, dto.AmountGb);
        return NoContent();
    }

    // DEPRECATED
    // NEED TO BE REMOVED
    [HttpGet("vm/vcenter-info")]
    public ActionResult GetVcenterInfo()
    {
        return Ok(new VCenterInfoDTO());
    }
}
