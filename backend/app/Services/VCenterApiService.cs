public class VCenterApiService
{
    private HttpClient _client;
    private readonly Config _config;

    public VCenterApiService(Config config)
    {
        _config = config;
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
        var credentials = $"{_config.VM_VCENTER_USER}:{_config.VM_VCENTER_PASSWORD}";
        var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Credentials);

        // Get and prepare access token
        HttpResponseMessage response = await _client.PostAsync($"https://{_config.VM_VCENTER_IP}/api/session", null);
        string responseBody = await response.Content.ReadAsStringAsync();
        string token = responseBody.Replace("\"", "");

        // set session token to header
        _client.DefaultRequestHeaders.Add("vmware-api-session-id", token);
    }

    public async Task<ResponseVCenterVmDto?> SearchVmByNameAsync(string vmName)
    {
        var vms = await _client.GetFromJsonAsync<List<ResponseVCenterVmDto>>($"https://{_config.VM_VCENTER_IP}/api/vcenter/vm?names={vmName}") ?? new List<ResponseVCenterVmDto>();
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

        var connectionUri = await _client.PostAsync($"https://{_config.VM_VCENTER_IP}/api/vcenter/vm/{selectedVmName.InternName}/console/tickets", content);
        ResponseVCenterTicketDto ticket = await connectionUri.Content.ReadFromJsonAsync<ResponseVCenterTicketDto>() ?? new ResponseVCenterTicketDto();
        ticket.VcenterIp = _config.VM_VCENTER_IP;

        return ticket;
    }

    public async Task<string> GetVmIpAsync(string internName)
    {
        string uri = $"https://{_config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/guest/networking/interfaces";

        var machines = await _client.GetFromJsonAsync<List<ResponseVmIpDto>>(uri) ?? new List<ResponseVmIpDto>();
        var selectedMachine = machines.FirstOrDefault(new ResponseVmIpDto());
        var ipv4 = selectedMachine.IP.IPAddresses.Where(x => x.PrefixLength == 24).FirstOrDefault(new ResponseVmIpDto.IPInfo.IPAddressInfo());

        return ipv4.IPAddress;
    }

    public async Task<int> GetCpuCount(string internName)
    {
        var uri = $"https://{_config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/hardware/cpu";
        var cpuCountData = await _client.GetFromJsonAsync<ResponseVmCpuDto>(uri) ?? new ResponseVmCpuDto();

        return cpuCountData.Count;
    }

    public async Task<int> GetRamSize(string internName)
    {
        var uri = $"https://{_config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/hardware/memory";
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