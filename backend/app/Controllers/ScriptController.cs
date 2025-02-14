[Route("script")]
[ApiController]
public class ScriptController(
    // VCenterApiService vCenterApiService,
    UserSession session, 
    VmBookingRepository vmBookingRepository,
    ProxmoxApiService proxmoxApiService,
    VmService vmService
    // VmBookingScriptService vmBookingScriptService
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

        // VmInfoGetDto? vmInfo = await vCenterApiService.GetInfo(name);
        // if (vmInfo == null)
        // {
        //     return NotFound(ResponseMessage.GetErrorMessage("Error under converting vm info"));
        // }

        // vmInfo.Username = booking.Login;
        // vmInfo.Password = booking.Password;
        
        return Ok(new VmInfoGetDto());
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

    // [HttpPut("vm/update-resources")]
    // public async Task<ActionResult> UpdateVmResources(VmUpdateResourcesDto dto)
    // {
    //     var user = session.GetIfRoles
    //     (
    //         Models.User.UserRoles.Admin,
    //         Models.User.UserRoles.Teacher
    //     );

    //     VmBooking? booking = await vmBookingRepository.GetByNameAsync(dto.Uuid);

    //     if (booking == null)
    //     {
    //         return NotFound(ResponseMessage.GetBookingNotFound());
    //     }

    //     // Deny access to the booking if the user is a teacher and it not assigned to booking
    //     if (user.IsTeacher() && booking.AssignedId != user.Id)
    //     {
    //         return NotFound(ResponseMessage.GetBookingNotFound());
    //     }

    //     // Make limits for resources
    //     if (dto.Cpu < 1 || dto.Cpu > 6)
    //     {
    //         return BadRequest(ResponseMessage.GetErrorMessage("Cpu must be between 1 and 6"));
    //     }

    //     if (dto.Ram < 2 || dto.Ram > 16)
    //     {
    //         return BadRequest(ResponseMessage.GetErrorMessage("Ram must be between 2 and 16"));
    //     }

    //     vmBookingScriptService.Update(booking.Name, dto.Cpu, dto.Ram);
    //     return NoContent();
    // }

    [HttpGet("vm/templates")]
    public async Task<ActionResult> GetTemplates()
    {
        session.IsAuthenticated();
        // return Ok(TemplateGetDto.MakeGetDtoFromList(TemplatesBackgroundService.GetTemplates()));
        return Ok(await proxmoxApiService.GetTemplates());
    }

    [HttpGet("proxmox/test")]
    public async Task<ActionResult> ProxmoxTest(int Id, string Name, string TemplateName)
    {
        // await proxmoxApiService.CloneVmByName(Id, Name, TemplateName);
        return Ok();
    }

    [HttpGet("proxmox/book")]
    public async Task<ActionResult> Book(int vmId, string name, string templateName)
    {
        _ = vmService.Book(name, templateName);
        return Ok();
    }

    [HttpGet("proxmox/delete")]
    public async Task<ActionResult> Delete(string name)
    {
        // _ = vmService.Book(vmId, name, templateName);
        _ = vmService.Remove(name);
        return Ok();
    }

    [HttpGet("proxmox/add-ha")]
    public async Task<ActionResult> AddHA(ProxmoxVmDto dto)
    {
        await proxmoxApiService.AddToHA((int)dto.VmId);
        return Ok();
    }

    // [HttpGet("proxmox/delete-ha")]
    // public async Task<ActionResult> DeleteHA(int Id)
    // {
    //     var vm = await proxmoxApiService.GetVmByIdAsync(Id);
    //     if (vm == null)
    //     {
    //         return NotFound();
    //     }

    //     var upid = await proxmoxApiService.DeleteFromHA(vm);
    //     System.Console.WriteLine($"VM stopped upid: {upid}");
    //     for (int i = 0; i < 600; i++)
    //     {
    //         var task = await proxmoxApiService.GetProxmoxTaskByUPID(upid);
    //         if (task == null)
    //         {
    //             System.Console.WriteLine("Task is null");
    //             break;
    //         }
    //         if (task.IsFinished == true && task.Successful == false)
    //         {
    //             System.Console.WriteLine("Task is not successful");
    //             return BadRequest();
    //         }

    //         if (task.IsFinished == true && task.Successful == true)
    //         {
    //             System.Console.WriteLine("Task is successful");
    //             break;
    //         }

    //         System.Console.WriteLine($"Waiting for task to finish seconds: {i}");
    //         await Task.Delay(1000);
    //     }

    //     upid = await proxmoxApiService.StopVm(vm);
    //     System.Console.WriteLine($"VM stopped upid: {upid}");
    //     for (int i = 0; i < 600; i++)
    //     {
    //         var task = await proxmoxApiService.GetProxmoxTaskByUPID(upid);
    //         if (task == null)
    //         {
    //             System.Console.WriteLine("Task is null");
    //             break;
    //         }
    //         if (task.IsFinished == true && task.Successful == false)
    //         {
    //             System.Console.WriteLine("Task is not successful");
    //             return BadRequest();
    //         }

    //         if (task.IsFinished == true && task.Successful == true)
    //         {
    //             System.Console.WriteLine("Task is successful");
    //             break;
    //         }

    //         System.Console.WriteLine($"Waiting for task to finish seconds: {i}");
    //         await Task.Delay(1000);
    //     }

    //     upid = await proxmoxApiService.DeleteVm(vm);
    //     System.Console.WriteLine($"VM deleted upid: {upid}");
    //     for (int i = 0; i < 600; i++)
    //     {
    //         var task = await proxmoxApiService.GetProxmoxTaskByUPID(upid);
    //         if (task == null)
    //         {
    //             break;
    //         }
    //         if (task.IsFinished == true && task.Successful == false)
    //         {
    //             return BadRequest();
    //         }

    //         if (task.IsFinished == true && task.Successful == true)
    //         {
    //             break;
    //         }

    //         System.Console.WriteLine($"Waiting for task to finish seconds: {i}");
    //         await Task.Delay(1000);
    //     }
    //     return Ok();
    // }

    [HttpGet("proxmox/tasks")]
    public async Task<ActionResult> GetTasks()
    {
        return Ok(await proxmoxApiService.GetProxmoxTasks());
    }

    [HttpGet("proxmox/task")]
    public async Task<ActionResult> GetTask(string upid)
    {
        return Ok(await proxmoxApiService.GetProxmoxTaskByUPID(upid));
    }

    [HttpGet("proxmox/vms")]
    public async Task<ActionResult> GetVms()
    {
        // read json from file
        string json = System.IO.File.ReadAllText("i.json");

        // deserialize json to object
        var tasks = JsonSerializer.Deserialize<List<ProxmoxTaskDto>>(json);

        return Ok(tasks);
    }

    // [HttpGet("vm/iso-list")]
    // public ActionResult GetIsoList()
    // {
    //     session.IsAuthenticated();
    //     return Ok(IsoBackgroundService.GetAllNames());
    // }

    // [HttpPost("vm/attach-iso")]
    // public async Task<ActionResult> AttachIso([FromBody] IsoAttachDto dto)
    // {
    //     User user = session.GetIfRoles
    //     (
    //         Models.User.UserRoles.Admin,
    //         Models.User.UserRoles.Teacher,
    //         Models.User.UserRoles.Student
    //     );

    //     VmBooking? booking = await vmBookingRepository.GetByNameAsync(dto.VmName);
    //     if (booking == null)
    //     {
    //         return NotFound(ResponseMessage.GetBookingNotFound());
    //     }

    //     Iso? iso = IsoBackgroundService.GetByName(dto.IsoName);
    //     if (iso == null)
    //     {
    //         return NotFound(ResponseMessage.GetErrorMessage("Iso not found"));
    //     }

    //     // Deny access to the booking if the user is a student and the booking is not his
    //     if (user.IsStudent() && booking.OwnerId != user.Id)
    //     {
    //         return NotFound(ResponseMessage.GetUserUnauthorized());
    //     }

    //     // Deny access to the booking if the user is a teacher and the booking not assigned to him
    //     if (user.IsTeacher() && booking.AssignedId != user.Id)
    //     {
    //         return NotFound(ResponseMessage.GetUserUnauthorized());
    //     }

    //     vmBookingScriptService.AttachIso(booking.Name, iso.Path);
    //     return NoContent();
    // }

    // [HttpPost("vm/attach-storage")]
    // public async Task<ActionResult> AttachStorage([FromBody] StorageAttachDto dto)
    // {
    //     User user = session.GetIfRoles
    //     (
    //         Models.User.UserRoles.Admin,
    //         Models.User.UserRoles.Teacher,
    //         Models.User.UserRoles.Student
    //     );

    //     VmBooking? booking = await vmBookingRepository.GetByNameAsync(dto.VmName);
    //     if (booking == null)
    //     {
    //         return NotFound(ResponseMessage.GetBookingNotFound());
    //     }

    //     // Deny access to the booking if the user is a student and the booking is not his
    //     if (user.IsStudent() && booking.OwnerId != user.Id)
    //     {
    //         return NotFound(ResponseMessage.GetUserUnauthorized());
    //     }

    //     // Deny access to the booking if the user is a teacher and the booking not assigned to him
    //     if (user.IsTeacher() && booking.AssignedId != user.Id)
    //     {
    //         return NotFound(ResponseMessage.GetUserUnauthorized());
    //     }

    //     // Deny access to the booking if amount of storage more than 500
    //     if (dto.AmountGb > 500)
    //     {
    //         return BadRequest(ResponseMessage.GetErrorMessage("Amount of storage more than 500"));
    //     }

    //     vmBookingScriptService.AttachStorage(booking.Name, dto.AmountGb);
    //     return NoContent();
    // }

    // [HttpPost("vm/detach-iso")]
    // public async Task<ActionResult> DetachIso([FromBody] IsoDetachDto dto)
    // {
    //     User user = session.GetIfRoles
    //     (
    //         Models.User.UserRoles.Admin,
    //         Models.User.UserRoles.Teacher,
    //         Models.User.UserRoles.Student
    //     );

    //     VmBooking? booking = await vmBookingRepository.GetByNameAsync(dto.VmName);
    //     if (booking == null)
    //     {
    //         return NotFound(ResponseMessage.GetBookingNotFound());
    //     }

    //     // Deny access to the booking if the user is a student and the booking is not his
    //     if (user.IsStudent() && booking.OwnerId != user.Id)
    //     {
    //         return NotFound(ResponseMessage.GetUserUnauthorized());
    //     }

    //     // Deny access to the booking if the user is a teacher and the booking not assigned to him
    //     if (user.IsTeacher() && booking.AssignedId != user.Id)
    //     {
    //         return NotFound(ResponseMessage.GetUserUnauthorized());
    //     }

    //     vmBookingScriptService.DetachIso(booking.Name);
    //     return NoContent();
    // }

    [HttpGet("vm/vcenter-info")]
    public ActionResult GetVcenterInfo()
    {
        // return Ok(VCenterInfoBackgroundService.GetInfo());
        return Ok(new VCenterInfoDTO());
    }
}
