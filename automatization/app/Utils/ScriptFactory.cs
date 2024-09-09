namespace Utils;

public class ScriptFactory(Config config)
{
    // VM scripts
    public ICommand GetTemlatesScript() {
        string scriptName = "get_templates.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetVmScript(string vmName)
    {
        string scriptName = "get_vm.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}' '{vmName}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetUpdateVmResourcesScript(string vmName, int cpu, int ram)
    {
        string scriptName = "update_vm_resources.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}' '{vmName}' {cpu} {ram}";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetCreateVmScript(string vmName, string templateName)
    {
        string scriptName = "create_vm_from_template.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{config.VM_VCENTER_USER}__{config.VM_VCENTER_PASSWORD}__{config.VM_VCENTER_IP}__{config.VM_CLUSTER_NAME}__{config.VM_DATASTORE_NAME}' '{templateName}' '{vmName}'";

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

    // Cluster scripts
    public ICommand GetCreateBackupScript(string hostIp, string hostUsername, string hostPassword) {
        string scriptName = "create_backup.sh";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{hostUsername}__{hostPassword}__{hostIp}'";

        return new ShellCommand($"{script} {args}");
    }

    public ICommand GetCreateClusterScript(string vcenterIp, string username, string password, string datacenterName, string clusterName) {
        string scriptName = "create_cluster.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{username}__{password}__{vcenterIp}' '{datacenterName}' '{clusterName}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetCreateDatacenterScript(string vcenterIp, string username, string password, string datacenterName) {
        string scriptName = "create_datacenter.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{username}__{password}__{vcenterIp}' '{datacenterName}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetCreateHostScript(string vcenterIp, string vcenterUsername, string vcenterPassword, string hostIp, string hostUsername, string hostPassword, string clusterName) {
        string scriptName = "create_host.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{vcenterUsername}__{vcenterPassword}__{vcenterIp}' '{hostUsername}__{hostPassword}__{hostIp}' '{clusterName}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetCreateVlanScript(string hostUsername, string hostPassword, string hostIp, int vlanNumber)
    {
        string scriptName = "create_vlan_tags.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{hostUsername}__{hostPassword}__{hostIp}' '{vlanNumber}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetMaintanceDisableScript(string hostUsername, string hostPassword, string hostIp)
    {
        string scriptName = "maintance_disable.sh";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{hostUsername}__{hostPassword}__{hostIp}'";

        return new ShellCommand($"{script} {args}");
    }

    public ICommand GetMaintanceEnableScript(string hostUsername, string hostPassword, string hostIp)
    {
        string scriptName = "maintance_enable.sh";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{hostUsername}__{hostPassword}__{hostIp}'";

        return new ShellCommand($"{script} {args}");
    }

    public ICommand GetRemoveVmsScript(string hostIp, string hostUsername, string hostPassword)
    {
        string scriptName = "remove_vms.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{hostUsername}__{hostPassword}__{hostIp}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetResetLicenseScript(string hostIp, string hostUsername, string hostPassword)
    {
        string scriptName = "reset_license.sh";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{hostUsername}__{hostPassword}__{hostIp}'";

        return new ShellCommand($"{script} {args}");
    }

    public ICommand GetRestoreBackupScript(string hostIp, string hostUsername, string hostPassword)
    {
        string scriptName = "restore_backup.sh";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{hostUsername}__{hostPassword}__{hostIp}'";

        return new ShellCommand($"{script} {args}");
    }

    public ICommand GetStopAndRemoveVmsScript(string hostIp, string hostUsername, string hostPassword)
    {
        string scriptName = "stop_and_remove_vms.ps1";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{hostUsername}__{hostPassword}__{hostIp}'";

        return new PowerShellCommand($"{script} {args}");
    }

    public ICommand GetInstallVcenterScript(string vcenterIp)
    {
        string scriptName = "install-vcenter.sh";

        string script = Path.Combine(config.SCRIPTS_PATH, scriptName);
        string args = $"'{vcenterIp}'";

        return new ShellCommand($"{script} {args}");
    }
}
