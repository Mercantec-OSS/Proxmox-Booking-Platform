namespace Services;

public class VmBookingScriptService()
{

    public void Create(string name, string template, string vmRootPassword, string vmUser, string vmPassword)
    {
        Script script = Script.GetCreateVmScript(name, template, vmRootPassword, vmUser, vmPassword);
        script.Run(false);
    }

    public void Remove(string name)
    {
        Script script = Script.GetRemoveVmScript(name);
        script.Run(false);
    }

    public List<string> GetTemplates()
    {
        Script script = Script.GetTemlatesScript();
        string rawOutput = script.Run(true);

        string[] templateNames = rawOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return templateNames.ToList();
    }

    public void Update(string vmName, int cpu, int ram)
    {
        Script script = Script.GetUpdateVmResourcesScript(vmName, cpu, ram);
        script.Run(false);
    }

    public void ResetPower(string vmName)
    {
        Script script = Script.GetResetVmPowerScript(vmName);
        script.Run(false);
    }

    public string GetVcenterInfo()
    {
        Script script = Script.GetVcenterInfoScript();
        return script.Run(true);
    }
}