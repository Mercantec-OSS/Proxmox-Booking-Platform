public class VCenterApiService
{
    private HttpClient _client;

    public VCenterApiService()
    {
        _client = InitHttpClient();
        Login().Wait();
    }

    private HttpClient InitHttpClient()
    {
        // Ignore SSL certificate
        HttpClientHandler handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        HttpClient client = new HttpClient(handler);

        return client;
    }

    private async System.Threading.Tasks.Task Login()
    {
        // Add authorization header to client
        var credentials = $"{Config.VM_VCENTER_USER}:{Config.VM_VCENTER_PASSWORD}";
        var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

        // Get and prepare access token
        HttpResponseMessage response = await _client.PostAsync($"https://{Config.VM_VCENTER_IP}/api/session", null);
        string responseBody = await response.Content.ReadAsStringAsync();
        string token = responseBody.Replace("\"", "");

        // set session token to header
        _client.DefaultRequestHeaders.Add("vmware-api-session-id", token);
    }

    public async Task<ResponseVCenterVmDto?> SearchVmByNameAsync(string vmName)
    {
        var vms = await _client.GetFromJsonAsync<List<ResponseVCenterVmDto>>($"https://{Config.VM_VCENTER_IP}/api/vcenter/vm?names={vmName}") ?? new List<ResponseVCenterVmDto>();
        return vms.FirstOrDefault();
    }

    public async Task<ResponseVCenterTicketDto> GetVmConnectionUriAsync(string vmName)
    {
        var selectedVmName = await SearchVmByNameAsync(vmName);
        if (selectedVmName == null)
        {
            return new ResponseVCenterTicketDto();
        }

        var data = new
        {
            type = "WEBMKS"
        };

        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var connectionUri = await _client.PostAsync($"https://{Config.VM_VCENTER_IP}/api/vcenter/vm/{selectedVmName.InternName}/console/tickets", content);
        ResponseVCenterTicketDto ticket = await connectionUri.Content.ReadFromJsonAsync<ResponseVCenterTicketDto>() ?? new ResponseVCenterTicketDto();
        ticket.VcenterIp = Config.VM_VCENTER_IP;

        return ticket;
    }

    public async Task<string> GetVmIpAsync(string internName)
    {
        string uri = $"https://{Config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/guest/networking/interfaces";

        var interfaces = await _client.GetFromJsonAsync<List<ResponseVmIpDto>>(uri) ?? new List<ResponseVmIpDto>();
        string machineIp = "";

        // Trow all machine interfaces
        foreach (var selectedInteface in interfaces)
        {
            var ipv4 = selectedInteface.IP.IPAddresses.Where(x => x.PrefixLength == 24).FirstOrDefault(new ResponseVmIpDto.IPInfo.IPAddressInfo());
            machineIp = ipv4.IPAddress;

            // If we found ipv4 with prefix 24, we can break the loop
            if (ipv4.PrefixLength == 24)
            {
                break;
            }
        }

        return machineIp;
    }

    public async Task<int> GetCpuCount(string internName)
    {
        var uri = $"https://{Config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/hardware/cpu";
        var cpuCountData = await _client.GetFromJsonAsync<ResponseVmCpuDto>(uri) ?? new ResponseVmCpuDto();

        return cpuCountData.Count;
    }

    public async Task<int> GetRamSize(string internName)
    {
        var uri = $"https://{Config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/hardware/memory";
        var ramSizeData = await _client.GetFromJsonAsync<ResponseVmRamDto>(uri) ?? new ResponseVmRamDto();

        return ramSizeData.SizeGB;
    }

    public async Task<VmInfoGetDto> GetInfo(string vmName)
    {
        var selectedVm = await SearchVmByNameAsync(vmName);
        if (selectedVm == null)
        {
            return new VmInfoGetDto();
        }

        string name = vmName;
        string ip = "";
        int cpuCores = 0;
        int ramGb = 0;

        try
        {
            ip = await GetVmIpAsync(selectedVm.InternName);
            cpuCores = await GetCpuCount(selectedVm.InternName);
            ramGb = await GetRamSize(selectedVm.InternName);
        }
        catch { }

        return new VmInfoGetDto()
        {
            Name = name,
            Ip = ip,
            Cpu = cpuCores,
            Ram = ramGb
        };
    }


}