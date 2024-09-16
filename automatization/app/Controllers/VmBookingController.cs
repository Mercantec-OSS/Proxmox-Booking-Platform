﻿namespace Controllers;

[ApiController]
[Route("vm-booking")]

public class VmBookingController(VmBookingService vmBookingService) : ControllerBase
{
    [HttpPost("create")]
    [ProducesResponseType(201)]
    public IActionResult Create(CreateVmDTO dto)
    {
        vmBookingService.Create(dto.Name, dto.Template, dto.RootPassword, dto.User, dto.Password);
        return Ok("VM created");
    }

    [HttpDelete("delete/{vmName}")]
    [ProducesResponseType(204)]
    public IActionResult Delete(string vmName)
    {
        vmBookingService.Remove(vmName);
        return NoContent();
    }

    [HttpGet("get-templates")]
    public ActionResult<List<string>> GetTemplates()
    {
        return Ok(TemplatesBackgroundService.GetTemplates());
    }

    [HttpGet("get-vm/{vmName}")]
    public ActionResult<VmDTO> GetVMResources(string vmName)
    {
        if (string.IsNullOrWhiteSpace(vmName))
        {
            return BadRequest("Invalid uuid");
        }

        VmDTO vm = vmBookingService.Get(vmName);
        return Ok(vm);
    }

    [HttpPut("update-vm")]
    [ProducesResponseType(204)]
    public IActionResult UpdateVMResources(UpdateVmDTO dto)
    {
        vmBookingService.Update(dto.Name, dto.Cpu, dto.Ram);
        return NoContent();
    }

    [HttpPost("reset-power/{vmName}")]
    [ProducesResponseType(204)]
    public IActionResult ResetVmPower(string vmName)
    {
        if (string.IsNullOrWhiteSpace(vmName))
        {
            return BadRequest("Invalid uuid");
        }

        vmBookingService.ResetPower(vmName);
        return NoContent();
    }

    [HttpGet("vcenter-info")]
    public ActionResult<VCenterInfoDTO> GetVcenterInfo()
    {
        return Ok(VCenterInfoBackgroundService.GetInfo());
    }

    [HttpGet("connection-uri/{vmName}")]
    public async Task<IActionResult> GetWebConsoleUri(Config config, string vmName)
    {
        var client = new VCenterApiService(config);
        return Ok(await client.GetVmConnectionUriAsync(vmName));
    }
}
