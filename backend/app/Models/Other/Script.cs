namespace Models;

public class Script
{
    public readonly string Command;
    public readonly string Interpreter;

    public Script(ScriptType scriptType, string command = "")
    {
        // define interpreter based on script type
        switch (scriptType)
        {
            case ScriptType.Bash:
                Interpreter = "/bin/bash";
                break;
            case ScriptType.PowerShell:
                Interpreter = "/usr/bin/pwsh";
                break;
            default:
                Interpreter = "/bin/bash";
                break;
        }

        this.Command = command;
    }

    public enum ScriptType
    {
        Bash,
        PowerShell
    }

    public static Script GetBashScript(string command)
    {
        return new Script(ScriptType.Bash, command);
    }

    public static Script GetPowerShellScript(string command)
    {
        return new Script(ScriptType.PowerShell, command);
    }

    public static Script GetTemlatesScript() {
        string scriptName = "get_templates.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}'";

        return GetPowerShellScript($"{script} {args}");
    }

    public static Script GetUpdateVmResourcesScript(string vmName, int cpu, int ram)
    {
        string scriptName = "update_vm_resources.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}' '{vmName}' {cpu} {ram}";

        return GetPowerShellScript($"{script} {args}");
    }

    public static Script GetCreateVmScript(string vmName, string templateName, string vmRootPassword, string vmUser, string vmPassword)
    {
        string scriptName = "create_vm_from_template.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}__{Config.VM_CLUSTER_NAME}__{Config.VM_DATASTORE_NAME}' '{templateName}' '{vmName}' '{vmRootPassword}' '{vmUser}' '{vmPassword}'";

        return GetPowerShellScript($"{script} {args}");
    }

    public static Script GetRemoveVmScript(string vmName)
    {
        string scriptName = "remove_vm.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}' '{vmName}'";

        return GetPowerShellScript($"{script} {args}");
    }

    public static Script GetResetVmPowerScript(string vmName)
    {
        string scriptName = "reset_vm_power.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}' '{vmName}'";

        return GetPowerShellScript($"{script} {args}");
    }

    public static Script GetVcenterInfoScript()
    {
        string scriptName = "get_vcenter_info.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}'";

        return GetPowerShellScript($"{script} {args}");
    }

    public static Script GetIsoListScript()
    {
        string scriptName = "get_iso_list.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}__{Config.VM_DATACENTER}'";

        return GetPowerShellScript($"{script} {args}");
    }

    public static Script GetAttachIsoScript(string vmName, string isoName)
    {
        string scriptName = "attach_iso.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}' '{vmName}' '{isoName}'";

        return GetPowerShellScript($"{script} {args}");
    }

    public static Script GetDetachIsoScript(string vmName)
    {
        string scriptName = "detach_iso.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}' '{vmName}'";

        return GetPowerShellScript($"{script} {args}");
    }
}
