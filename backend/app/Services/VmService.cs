public class VmService(ProxmoxApiService proxmoxApiService)
{
    public async Task Book(string vmName, string templateName, string username = "", string password = "") {
        int vmId = await GetFreeVmId();

        ProxmoxVmDto? template = await proxmoxApiService.GetTemplateByNameAsync(templateName);
        ProxmoxNodeDto? node = await proxmoxApiService.GetProxmoxNodeForBooking();

        if (node == null)
        {
            throw new Exception("Not available nodes");
        }

        if (template == null)
        {
            throw new HttpException(HttpStatusCode.NotFound,"Template not found");
        }

        if (await proxmoxApiService.CloneVm(vmId, vmName, template, node, true) == false) 
        {
            throw new Exception("Failed to clone vm");
        }

        await proxmoxApiService.AddToHA(vmId);
        await WaitForAgent(vmName);

        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        if (username != "") {
            await proxmoxApiService.SetVmPassword(vm, username, password);
        }
    }

    public async Task<ProxmoxVmDto> GetTemplateByNameAsync(string name) {
        ProxmoxVmDto? template = await proxmoxApiService.GetTemplateByNameAsync(name);
        if (template == null)
        {
            throw new HttpException(HttpStatusCode.NotFound,"Template not found");
        }

        return template;
    }

    public async Task<ProxmoxVmDto> GetVmByNameAsync(string name) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(name);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        return vm;
    }

    public async Task Remove(string vmName) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        await proxmoxApiService.DeleteFromHA(vm);
        await proxmoxApiService.StopVm(vm, true);
        await proxmoxApiService.DeleteVm(vm);
    }

    public async Task ResetPower(string vmName) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        await proxmoxApiService.ResetVmPower(vm);
    }

    public async Task RebootVm(string vmName) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        await proxmoxApiService.RebootVm(vm);
    }


    public async Task<List<ProxmoxIsoDto>> GetPromoxIsoList() {
        List<ProxmoxNodeDto> nodes = await proxmoxApiService.GetProxmoxNodes();
        if (nodes.Count == 0)
        {
            throw new Exception("No nodes found. Can not get iso list");
        }

        return await proxmoxApiService.GetProxmoxIsoList(nodes.First());
    }

    public async Task<List<IsoDto>> GetIsoList() {
        List<ProxmoxIsoDto> proxmoxIsos = await GetPromoxIsoList();
        return proxmoxIsos.ConvertAll(IsoDto.GetFromProxmoxIso);
    }

    public async Task<ProxmoxIsoDto> GetProxmoxIsoByName(string name) {
        List<ProxmoxIsoDto> proxmoxIsoList = await GetPromoxIsoList();
        ProxmoxIsoDto? iso = proxmoxIsoList.FirstOrDefault(iso => iso.Name == name);
        if (iso == null)
        {
            throw new Exception("Iso not found");
        }
        
        return iso;
    }

    public async Task AttachIso(string vmName, string isoName) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        ProxmoxIsoDto iso = await GetProxmoxIsoByName(isoName);
        await proxmoxApiService.AttachIso(vm, iso);
    }

    public async Task DetachIso(string vmName) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        await proxmoxApiService.DetachIso(vm);
    }

    public async Task AddStorage(string vmName, int sizeGb) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        ProxmoxVmConfigDto vmConfig = await proxmoxApiService.GetVmConfig(vm);
        if (vmConfig.ExtraStorageExists)
        {
            throw new HttpException(HttpStatusCode.NotAcceptable, "Extra storage already exists");
        }

        _ = proxmoxApiService.AddStorage(vm, sizeGb);
    }

    public async Task UpdateVmResources(string vmName, int cpu, int ramMb) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new HttpException(HttpStatusCode.NotFound, "Vm not found");
        }

        await proxmoxApiService.UpdateVmConfig(vm, cpu, ramMb);
        await proxmoxApiService.StopVm(vm, true);
        await proxmoxApiService.StartVm(vm);
    }

    public async Task<bool> WaitForAgent(string vmName) {
        bool result = false;
        
        for (int i = 0; i < 300; i++)
        {
            ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
            if (vm == null)
            {
                break;
            }

            bool vmStatus = await proxmoxApiService.GetAgentStatus(vm);
            await Task.Delay(1000);

            if (vmStatus)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private async Task<int> GetFreeVmId() {
        int vmId = Helpers.GetRandomNumber(10000, 99999);

        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByIdAsync(vmId);
        if (vm == null)
        {
            return vmId;
        }

        return await GetFreeVmId();
    }
}