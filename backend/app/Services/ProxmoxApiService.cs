public class ProxmoxApiService
{
    private HttpClient _client;

    public ProxmoxApiService()
    {
        _client = InitHttpClient();
    }

    public async Task<List<T>> GetProxmoxResources<T>(string type)
    {
        var response = await _client.GetAsync($"https://{Config.PROXMOX_ADDR}/api2/json/cluster/resources?type={type}");

        // get data from json response
        var data = await response.Content.ReadFromJsonAsync<Dictionary<string, List<T>>>();
        if (data == null)
        {
            return new List<T>();
        }

        return data["data"];
    }

    // tasks
    public async Task<List<ProxmoxTaskDto>> GetProxmoxTasks()
    {
        var response = await _client.GetAsync($"https://{Config.PROXMOX_ADDR}/api2/json/cluster/tasks");
        Dictionary<string, List<ProxmoxTaskDto>> data;
        var respData = await response.Content.ReadFromJsonAsync<Dictionary<string, List<ProxmoxTaskDto>>>();

        if (respData == null)
        {
            return new List<ProxmoxTaskDto>();
        }

        data = respData;
        return data["data"];
    }

    public async Task<ProxmoxTaskDto?> GetProxmoxTaskByUPID(string upid)
    {
        List<ProxmoxTaskDto> tasks = await GetProxmoxTasks();
        return tasks.FirstOrDefault(x => x.Upid == upid);
    }

    // templates
    public async Task<List<TemplateGetDto>> GetTemplates()
    {
        List<ProxmoxVmDto> proxmoxTemplates = await GetProxmoxTemplates();
        return proxmoxTemplates.ConvertAll(x => TemplateGetDto.MakeGetDTO(x.Name));
    }

    public async Task<List<ProxmoxVmDto>> GetProxmoxTemplates()
    {
        List<ProxmoxVmDto> vmsAndTemplates = await GetProxmoxResources<ProxmoxVmDto>("vm");
        return vmsAndTemplates.Where(x => x.Template == 1).ToList();
    }

    public async Task<ProxmoxVmDto?> GetTemplateByNameAsync(string name)
    {
        List<ProxmoxVmDto> templates = await GetProxmoxTemplates();
        return templates.FirstOrDefault(x => x.Name == name);
    }
    // VMs
    public async Task<List<ProxmoxVmDto>> GetProxmoxVms()
    {
        List<ProxmoxVmDto> vmsAndTemplates = await GetProxmoxResources<ProxmoxVmDto>("vm");
        return vmsAndTemplates.Where(x => x.Template == 0).ToList();
    }

    public async Task<ProxmoxVmDto?> GetVmByNameAsync(string name)
    {
        List<ProxmoxVmDto> vms = await GetProxmoxVms();
        return vms.FirstOrDefault(x => x.Name == name);
    }

    public async Task<ProxmoxVmDto?> GetVmByIdAsync(int id)
    {
        List<ProxmoxVmDto> vms = await GetProxmoxVms();
        return vms.FirstOrDefault(x => x.VmId == id);
    }

    public async Task<bool> CloneVm(int vmId, string vmName, ProxmoxVmDto template, ProxmoxNodeDto node, bool waitTask = false)
    {
        var data = new
        {
            newid = vmId,
            target = node.Node,
            full = 1,
            name = vmName
        };

        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"https://{Config.PROXMOX_ADDR}/api2/json/nodes/{template.Node}/qemu/{template.VmId}/clone", content);
        string upid = await ReadTaskUPID(response);

        if (waitTask)
        {
            return await WaitForTaskFinish(upid);
        }

        return true;
    }

    public async Task<bool> StopVm(ProxmoxVmDto vm, bool waitTask = false)
    {
        var response = await _client.PostAsync($"https://{Config.PROXMOX_ADDR}/api2/json/nodes/{vm.Node}/qemu/{vm.VmId}/status/stop", new StringContent(""));
        string upid = await ReadTaskUPID(response);

        if (waitTask)
        {
            return await WaitForTaskFinish(upid);
        }

        return true;
    }

    public async Task<bool> DeleteVm(ProxmoxVmDto vm, bool waitTask = false)
    {
        var response = await _client.DeleteAsync($"https://{Config.PROXMOX_ADDR}/api2/json/nodes/{vm.Node}/qemu/{vm.VmId}?destroy-unreferenced-disks=1&purge=1");
        string upid = await ReadTaskUPID(response);

        if (waitTask)
        {
            return await WaitForTaskFinish(upid);
        }

        return true;
    }

    // Nodes
    public async Task<List<ProxmoxNodeDto>> GetProxmoxNodes()
    {
        return await GetProxmoxResources<ProxmoxNodeDto>("node");
    }

    public async Task<ProxmoxNodeDto?> GetProxmoxNode(string name)
    {
        List<ProxmoxNodeDto> nodes = await GetProxmoxNodes();
        return nodes.FirstOrDefault(x => x.Node == name);
    }

    public async Task<ProxmoxNodeDto?> GetProxmoxNodeForBooking()
    {
        List<ProxmoxNodeDto> nodes = await GetProxmoxNodes();
        List<ProxmoxNodeDto> availableNodes = nodes.Where(x => x.ReadyForBookings == true).ToList();
        return availableNodes.OrderBy(x => x.LoadIndex).FirstOrDefault();
    }

    // HA
    public async Task<bool> AddToHA(int vmId, bool waitTask = false)
    {
        var data = new
        {
            sid = vmId
        };

        var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync($"https://{Config.PROXMOX_ADDR}/api2/json/cluster/ha/resources", content);
        string upid = await ReadTaskUPID(response);

        if (waitTask)
        {
            return await WaitForTaskFinish(upid);
        }

        return true;
    }

    public async Task<bool> DeleteFromHA(ProxmoxVmDto vm, bool waitTask = false)
    {
        var response = await _client.DeleteAsync($"https://{Config.PROXMOX_ADDR}/api2/json/cluster/ha/resources/{vm.VmId}");
        string upid = await ReadTaskUPID(response);

        if (waitTask)
        {
            return await WaitForTaskFinish(upid);
        }

        return true;
    }


    // Other
    private HttpClient InitHttpClient()
    {
        // Ignore SSL certificate
        HttpClientHandler handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        HttpClient client = new HttpClient(handler);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("PVEAPIToken", $"{Config.PROXMOX_TOKEN_ID}={Config.PROXMOX_TOKEN_SECRET}");

        return client;
    }

    private async Task<string> ReadTaskUPID(HttpResponseMessage response)
    {
        var data = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        if (data == null)
        {
            return "";
        }

        return data["data"];
    }

    private async Task<bool> WaitForTaskFinish(string upid, int timeoutSeconds = 300)
    {
        ProxmoxTaskDto? task = await GetProxmoxTaskByUPID(upid);
        for (int i = 0; i < timeoutSeconds; i++)
        {
            await Task.Delay(2000);

            if (task == null)
            {
                Console.WriteLine($"{upid}\tTask is null");
                return false;
            }

            if (task.IsFinished == true && task.Successful == false)
            {
                Console.WriteLine($"{upid}\tTask is not successful");
                return false;
            }

            if (task.IsFinished == true && task.Successful == true)
            {
                return true;
            }
            task = await GetProxmoxTaskByUPID(upid);
        }

        Console.WriteLine($"{upid}\tTask timeout error");
        return false;
    }



    // public async Task<ResponseVCenterVmDto?> SearchVmByNameAsync(string vmName)
    // {

    // var vms = await _client.GetFromJsonAsync<List<ResponseVCenterVmDto>>($"https://{Config.VM_VCENTER_IP}/api/vcenter/vm?names={vmName}") ?? new List<ResponseVCenterVmDto>();
    // return vms.FirstOrDefault();
    // }

    //     public async Task<ResponseVCenterTicketDto> GetVmConnectionUriAsync(string vmName)
    //     {
    //         var selectedVmName = await SearchVmByNameAsync(vmName);
    //         if (selectedVmName == null)
    //         {
    //             return new ResponseVCenterTicketDto();
    //         }

    //         var data = new
    //         {
    //             type = "WEBMKS"
    //         };

    //         var json = JsonSerializer.Serialize(data);
    //         var content = new StringContent(json, Encoding.UTF8, "application/json");

    //         var connectionUri = await _client.PostAsync($"https://{Config.VM_VCENTER_IP}/api/vcenter/vm/{selectedVmName.InternName}/console/tickets", content);
    //         ResponseVCenterTicketDto ticket = await connectionUri.Content.ReadFromJsonAsync<ResponseVCenterTicketDto>() ?? new ResponseVCenterTicketDto();
    //         ticket.VcenterIp = Config.VM_VCENTER_IP;

    //         return ticket;
    //     }

    //     public async Task<string> GetVmIpAsync(string internName)
    //     {
    //         string uri = $"https://{Config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/guest/networking/interfaces";

    //         var interfaces = await _client.GetFromJsonAsync<List<ResponseVmIpDto>>(uri) ?? new List<ResponseVmIpDto>();
    //         string machineIp = "";

    //         // Trow all machine interfaces
    //         foreach (var selectedInteface in interfaces)
    //         {
    //             var ipv4 = selectedInteface.IP.IPAddresses.Where(x => x.PrefixLength == 24).FirstOrDefault(new ResponseVmIpDto.IPInfo.IPAddressInfo());
    //             machineIp = ipv4.IPAddress;

    //             // If we found ipv4 with prefix 24, we can break the loop
    //             if (ipv4.PrefixLength == 24)
    //             {
    //                 break;
    //             }
    //         }

    //         return machineIp;
    //     }

    //     public async Task<int> GetCpuCount(string internName)
    //     {
    //         var uri = $"https://{Config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/hardware/cpu";
    //         var cpuCountData = await _client.GetFromJsonAsync<ResponseVmCpuDto>(uri) ?? new ResponseVmCpuDto();

    //         return cpuCountData.Count;
    //     }

    //     public async Task<int> GetRamSize(string internName)
    //     {
    //         var uri = $"https://{Config.VM_VCENTER_IP}/api/vcenter/vm/{internName}/hardware/memory";
    //         var ramSizeData = await _client.GetFromJsonAsync<ResponseVmRamDto>(uri) ?? new ResponseVmRamDto();

    //         return ramSizeData.SizeGB;
    //     }

    //     public async Task<VmInfoGetDto> GetInfo(string vmName)
    //     {
    //         var selectedVm = await SearchVmByNameAsync(vmName);
    //         if (selectedVm == null)
    //         {
    //             return new VmInfoGetDto();
    //         }

    //         string name = vmName;
    //         string ip = "";
    //         int cpuCores = 0;
    //         int ramGb = 0;

    //         try
    //         {
    //             ip = await GetVmIpAsync(selectedVm.InternName);
    //             cpuCores = await GetCpuCount(selectedVm.InternName);
    //             ramGb = await GetRamSize(selectedVm.InternName);
    //         }
    //         catch { }

    //         return new VmInfoGetDto()
    //         {
    //             Name = name,
    //             Ip = ip,
    //             Cpu = cpuCores,
    //             Ram = ramGb
    //         };
    //     }


}