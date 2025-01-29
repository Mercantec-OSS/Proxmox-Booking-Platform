public class VmService(ProxmoxApiService proxmoxApiService)
{
    public async Task Book(string vmName, string templateName) {
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

    private async Task<int> GetFreeVmId() {
        Random random = new Random();
        int vmId = random.Next(10000, 99999);

        ProxmoxVmDto? vm = await proxmoxApiService.GetVmByIdAsync(vmId);
        if (vm == null)
        {
            return vmId;
        }

        return await GetFreeVmId();
    }
}