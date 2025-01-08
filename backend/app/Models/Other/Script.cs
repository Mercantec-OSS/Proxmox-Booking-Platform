using System.Text.RegularExpressions;


namespace Models;

public class Script
{
    private readonly string command;
    private readonly string interpreter;

    public Script(ScriptType scriptType, string command = "")
    {
        // define interpreter based on script type
        switch (scriptType)
        {
            case ScriptType.Bash:
                interpreter = "/bin/bash";
                break;
            case ScriptType.PowerShell:
                interpreter = "/usr/bin/pwsh";
                break;
            default:
                interpreter = "/bin/bash";
                break;
        }

        this.command = command;
    }
    public string CommandString => Regex.Replace(command, @"'([^']*)'", "x");

    public string Run(bool readOutput)
    {
        return Execute(command, readOutput);
    }
    private string Execute(string command, bool readOutput)
    {
        string output = "";

        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = interpreter,
            Arguments = $"-c \"{command}\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
        };

        using (Process process = new Process())
        {
            try
            {
                process.StartInfo = processStartInfo;
                process.Start();

                if (readOutput)
                {
                    output = process.StandardOutput.ReadToEnd();
                }
                else {
                    output = "Command was started";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing command: " + ex.Message);
                output = $"Error executing command: {ex.Message}";
            }
        }

        return output;
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

    // Other scripts
    public static Script GetVcenterInfoScript()
    {
        string scriptName = "get_vcenter_info.ps1";

        string script = Path.Combine(Config.SCRIPTS_PATH, scriptName);
        string args = $"'{Config.VM_VCENTER_USER}__{Config.VM_VCENTER_PASSWORD}__{Config.VM_VCENTER_IP}'";

        return GetPowerShellScript($"{script} {args}");
    }
}
