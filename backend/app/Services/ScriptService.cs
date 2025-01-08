namespace Services;

public class ScriptService
{
    private readonly HttpClient _httpClient;
    private readonly Config _config;

    public ScriptService(Config config)
    {
        _config = config;
        _httpClient = new()
        {
            Timeout = TimeSpan.FromMinutes(5),
            BaseAddress = new(_config.DEVOPS_URL)
        };
    }

    // Utils
    private async Task<string> GetAsync(string url)
    {
        if (_config.TEST_MODE)
        {
            Console.WriteLine("TEST MODE. Skipping script execution");
            return "TEST MODE";
        }

        var response = await _httpClient.GetAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<string> DeleteAsync(string url)
    {
        if (_config.TEST_MODE)
        {
            Console.WriteLine("TEST MODE. Skipping script execution");
            return "TEST MODE";
        }
        
        var response = await _httpClient.DeleteAsync(url);
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<string> PostAsync(string url, StringContent content)
    {
        if (_config.TEST_MODE)
        {
            Console.WriteLine("TEST MODE. Skipping script execution");
            return "TEST MODE";
        }
        
        var response = await _httpClient.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    private async Task<string> PutAsync(string url, StringContent content)
    {
        if (_config.TEST_MODE)
        {
            Console.WriteLine("TEST MODE. Skipping script execution");
            return "TEST MODE";
        }
        
        var response = await _httpClient.PutAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    // VM operations
    public async Task<string> CreateVmAsync(string vmType, string vmName, string rootPassword, string user, string password)
    {
        string url = $"vm-booking/create";
        var data = new 
        {
            name = vmName,
            template = vmType,
            rootPassword,
            user,
            password
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await PostAsync(url, content);
    }

    public async Task<string> DeleteVmAsync(string vmName)
    {
        string url = $"vm-booking/delete/{vmName}";
        return await DeleteAsync(url);
    }

    public async Task<VmInfoGetDto?> GetVmAsync(string vmName)
    {
        string url = $"vm-booking/get-vm/{vmName}";

        string responseText = await GetAsync($"{url}");
        var data = JsonSerializer.Deserialize<VmInfoGetDto>(responseText);

        return data;
    }

    public async Task<VmConnectionUriDto?> GetVmConnectionUriAsync(string vmName)
    {
        string url = $"vm-booking/connection-uri/{vmName}";

        string responseText = await GetAsync($"{url}");
        var data = JsonSerializer.Deserialize<VmConnectionUriDto>(responseText);

        return data;
    }

    public async Task<string> ResetPowerVmAsync(string vmName)
    {
        string url = $"vm-booking/reset-power/{vmName}";
        return await PostAsync(url, new StringContent(""));
    }

    public async Task<List<TemplateGetDto>> GetTemplatesAsync()
    {
        string response = await GetAsync("vm-booking/get-templates");
        List<string> templatesRaw = JsonSerializer.Deserialize<List<string>>(response) ?? new();
        return templatesRaw.ConvertAll(TemplateGetDto.MakeGetDTO);
    }

    public async Task ResetTemplatesAsync()
    {
        await DeleteAsync("vm-booking/reset-templates");
    }

    public async Task<VCenterInfoDTO> GetVcenterInfoAsync()
    {
        string response = await GetAsync("vm-booking/vcenter-info");
        VCenterInfoDTO data = JsonSerializer.Deserialize<VCenterInfoDTO>(response) ?? new();

        return data;
    }
}