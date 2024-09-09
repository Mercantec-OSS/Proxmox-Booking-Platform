using System.Net.Http.Headers;
using System.Text;

namespace Controllers;

[ApiController]
[Route("vm-booking")]

public class VmBookingController(VmBookingService vmBookingService) : ControllerBase
{
    private readonly List<string> _templateCache = new ();
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
        return Ok(_templateCache);
    }

    [HttpGet("update-templates-cache")]
    [ProducesResponseType(204)]
    public IActionResult ClearTemplatesCache()
    {
        _templateCache.AddRange(vmBookingService.GetTemplates());
        _templateCache.Clear();
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
