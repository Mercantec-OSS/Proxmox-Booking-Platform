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
            throw new Exception("Template not found");
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
            throw new Exception("Vm not found");
        }

        if (username != "") {
            await proxmoxApiService.SetVmPassword(vm, username, password);
        }
    }

    public async Task<ProxmoxVmDto> GetTemplateByNameAsync(string name) {
        ProxmoxVmDto? template = await proxmoxApiService.GetTemplateByNameAsync(name);
        if (template == null)
        {
            throw new Exception("Template not found");
        }

        return template;
    }

    public async Task<ProxmoxVmDto> GetVmByNameAsync(string name) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(name);
        if (vm == null)
        {
            throw new Exception("VM not found");
        }

        return vm;
    }

    public async Task Remove(string vmName) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new Exception("Vm not found");
        }

        await proxmoxApiService.DeleteFromHA(vm);
        await proxmoxApiService.StopVm(vm, true);
        await proxmoxApiService.DeleteVm(vm);
    }

    public async Task ResetPower(string vmName) {
        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByNameAsync(vmName);
        if (vm == null)
        {
            throw new Exception("Vm not found");
        }

        await proxmoxApiService.ResetVmPower(vm);
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