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

    // Cluster operations
    public async Task<string> ResetLicenseAsync(string hostUsername, string hostPassword, string hostIp, string afterTask = "")
    {
        string url = $"cluster-booking/reset-license";

        var query = HttpUtility.ParseQueryString("");
        query["hostUsername"] = hostUsername;
        query["hostPassword"] = hostPassword;
        query["hostIp"] = hostIp;
        query["afterThan"] = afterTask;

        return await GetAsync($"{url}?{query}");
    }

    public async Task<string> CreateBackupAsync(string hostUsername, string hostPassword, string hostIp, string datastoreName, string afterTask = "")
    {
        string url = $"cluster-booking/create-backup";

        var query = HttpUtility.ParseQueryString("");
        query["hostUsername"] = hostUsername;
        query["hostPassword"] = hostPassword;
        query["hostIp"] = hostIp;
        query["readOutput"] = "true";
        query["datastoreName"] = datastoreName;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> RestoreBackupAsync(string hostUsername, string hostPassword, string hostIp, string afterTask = "")
    {
        string url = $"cluster-booking/restore-backup";

        var query = HttpUtility.ParseQueryString("");
        query["hostUsername"] = hostUsername;
        query["hostPassword"] = hostPassword;
        query["hostIp"] = hostIp;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> MaintanceEnableAsync(string hostUsername, string hostPassword, string hostIp, string afterTask = "")
    {
        string url = $"cluster-booking/maintance-enable";

        var query = HttpUtility.ParseQueryString("");
        query["hostUsername"] = hostUsername;
        query["hostPassword"] = hostPassword;
        query["hostIp"] = hostIp;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> MaintanceDisableAsync(string hostUsername, string hostPassword, string hostIp, string afterTask = "")
    {
        string url = $"cluster-booking/maintance-disable";

        var query = HttpUtility.ParseQueryString("");
        query["hostUsername"] = hostUsername;
        query["hostPassword"] = hostPassword;
        query["hostIp"] = hostIp;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> InstallVCenterAsync(string jsonConfig, string afterTask = "")
    {
        string url = $"cluster-booking/install-vcenter";
        string base64Config = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonConfig));

        var query = HttpUtility.ParseQueryString("");
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent($"\"{base64Config}\"", Encoding.UTF8, "application/json"));
    }

    public async Task<string> CreateDatacenterAsync(string vcenterUsername, string vcenterPassword, string vcenterIp, string datacenterName, string afterTask = "")
    {
        string url = $"cluster-booking/create-datacenter";

        var query = HttpUtility.ParseQueryString("");
        query["vcenterUsername"] = vcenterUsername;
        query["vcenterPassword"] = vcenterPassword;
        query["vcenterIp"] = vcenterIp;
        query["datacenterName"] = datacenterName;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> CreateClusterAsync(string vcenterUsername, string vcenterPassword, string vcenterIp, string datacenterName, string clusterName, string afterTask = "")
    {
        string url = $"cluster-booking/create-cluster";

        var query = HttpUtility.ParseQueryString("");
        query["vcenterUsername"] = vcenterUsername;
        query["vcenterPassword"] = vcenterPassword;
        query["vcenterIp"] = vcenterIp;
        query["datacenterName"] = datacenterName;
        query["clusterName"] = clusterName;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> CreateHostAsync(string vcenterUsername, string vcenterPassword, string vcenterIp, string clusterName, string hostUsername, string hostPassword, string hostIp, string afterTask = "")
    {
        string url = $"cluster-booking/create-host";

        var query = HttpUtility.ParseQueryString("");
        query["vcenterUsername"] = vcenterUsername;
        query["vcenterPassword"] = vcenterPassword;
        query["vcenterIp"] = vcenterIp;
        query["hostUsername"] = hostUsername;
        query["hostPassword"] = hostPassword;
        query["hostIp"] = hostIp;
        query["clusterName"] = clusterName;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> StopAndRemoveVMSAsync(string hostUsername, string hostPassword, string hostIp, string afterTask = "")
    {
        string url = $"cluster-booking/stop-and-remove-vms";

        var query = HttpUtility.ParseQueryString("");
        query["hostUsername"] = hostUsername;
        query["hostPassword"] = hostPassword;
        query["hostIp"] = hostIp;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> RunShellAsync(string command, string afterTask = "")
    {
        string url = $"cluster-booking/shell-command";

        var query = HttpUtility.ParseQueryString("");
        query["command"] = command;
        query["afterThan"] = afterTask;

        return await PostAsync($"{url}?{query}", new StringContent(""));
    }

    public async Task<string> CreateClusterBooking(List<VCenter> vcenters, string afterTask = "")
    {
        string lastTaskUuid = afterTask;
        foreach (var vcenter in vcenters)
        {
            var taskUuid = await InstallVCenterAsync(vcenter.JsonConfig, afterTask);
            taskUuid = await CreateDatacenterAsync(vcenter.UserName, vcenter.Password, vcenter.Ip, vcenter.DatacenterName, taskUuid);
            taskUuid = await CreateClusterAsync(vcenter.UserName, vcenter.Password, vcenter.Ip, vcenter.DatacenterName, vcenter.ClusterName, taskUuid);

            foreach (var host in vcenter.EsxiHosts)
            {
                lastTaskUuid = await CreateHostAsync(vcenter.UserName, vcenter.Password, vcenter.Ip, vcenter.ClusterName, host.UserName, host.Password, host.Ip, taskUuid);
            }
        }

        return lastTaskUuid;
    }

    public async Task<string> ResetHostAsync(EsxiHost host, string afterTask = "")
    {
        string taskUuid = afterTask;
        taskUuid = await ResetLicenseAsync(host.UserName, host.Password, host.Ip, afterTask);

        taskUuid = await StopAndRemoveVMSAsync(host.UserName, host.Password, host.Ip, taskUuid);
        taskUuid = await MaintanceEnableAsync(host.UserName, host.Password, host.Ip, taskUuid);
        taskUuid = await RestoreBackupAsync(host.UserName, host.Password, host.Ip, taskUuid);

        return taskUuid;
    }

    public async Task<string> ResetClusterBookingAsync(List<VCenter> vcenters, string afterTask = "")
    {
        string taskUuid = "";
        foreach (EsxiHost host in vcenters.SelectMany(vcenter => vcenter.EsxiHosts))
        {
            taskUuid = await ResetHostAsync(host, afterTask);
        }

        return taskUuid;
    }

    public async Task<string> ResetAndInstallVcenterAsync(List<VCenter> vcenters)
    {
        // reset task
        string lastTaskUuid = await ResetClusterBookingAsync(vcenters);

        // sleep for await servers restart
        lastTaskUuid = await RunShellAsync("sleep 600", lastTaskUuid);

        // install
        lastTaskUuid = await CreateClusterBooking(vcenters, lastTaskUuid);

        return lastTaskUuid;
    }
}