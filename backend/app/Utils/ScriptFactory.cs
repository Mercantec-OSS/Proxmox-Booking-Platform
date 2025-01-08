public class ScriptFactory(Config config)
{
    // VM scripts
    public ICommand GetTemlatesScript() {
        string scriptName = "get_templates.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetUpdateVmResourcesScript(string vmName, int cpu, int ram)
    {
        string scriptName = "update_vm_resources.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}' '{vmName}' {cpu} {ram}";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetCreateVmScript(string vmName, string templateName, string vmRootPassword, string vmUser, string vmPassword)
    {
        string scriptName = "create_vm_from_template.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}__{config.VM_CLUSTER_NAME}__{config.VM_DATASTORE_NAME}' '{templateName}' '{vmName}' '{vmRootPassword}' '{vmUser}' '{vmPassword}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetRemoveVmScript(string vmName)
    {
        string scriptName = "remove_vm.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}' '{vmName}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetResetVmPowerScript(string vmName)
    {
        string scriptName = "reset_vm_power.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}' '{vmName}'";

        return new PowerShellCommand($"{script} {args}");
    }

    // Other scripts
    public ICommand GetVcenterInfoScript()
    {
        string scriptName = "get_vcenter_info.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetRemoveJsonConfigScript(string pathFile)
    {
        return new ShellCommand($"rm -rf {pathFile}");
    }
}
