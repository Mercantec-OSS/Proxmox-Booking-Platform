namespace Controllers;

[ApiController]
[Route("vm-booking")]

public class VmBookingController(VmBookingService vmBookingService) : ControllerBase
{
    private static readonly List<string> _templates = new ();
    [HttpPost("create")]
    [ProducesResponseType(201)]
    public IActionResult Create(CreateVmDTO dto)
    {
        vmBookingService.Create(dto.Name, dto.Template);
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
        if (_templates.Count == 0)
        {
            _templates.AddRange(vmBookingService.GetTemplates());
        }

        return Ok(_templates);
    }

    [HttpDelete("delete-templates")]
    [ProducesResponseType(204)]
    public IActionResult DeleteTemplates()
    {
        _templates.Clear();
        return NoContent();
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

    [HttpGet("test")]
    public async Task<IActionResult> Test(Config config)
    {
        string host = config.VM_VCENTER_IP;
        string username = config.VM_VCENTER_USER;
        string password = config.VM_VCENTER_PASSWORD;

        // Створення базової авторизації
        var credentials = $"{username}:{password}";
        var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));

        // Ігнорування SSL сертифікату
        HttpClientHandler handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        HttpClient client = new HttpClient(handler);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

        HttpResponseMessage response = await client.PostAsync($"https://{host}/api/session", null);

        string responseBody = await response.Content.ReadAsStringAsync();
        string token = responseBody.Replace("\"", "");

        client.DefaultRequestHeaders.Add("vmware-api-session-id", token);

        return Ok(token);
    }
}
