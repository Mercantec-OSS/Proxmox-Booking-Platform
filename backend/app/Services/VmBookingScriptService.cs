namespace Services;

public class VmBookingScriptService(
    ScriptService scriptService
)
{

    public void Create(string name, string template, string vmRootPassword, string vmUser, string vmPassword)
    {
        Script script = Script.GetCreateVmScript(name, template, vmRootPassword, vmUser, vmPassword);
        scriptService.Execute(script, false);
    }

    public void Remove(string name)
    {
        Script script = Script.GetRemoveVmScript(name);
        scriptService.Execute(script, false);
    }

    public List<string> GetTemplates()
    {
        Script script = Script.GetTemlatesScript();
        string rawOutput = scriptService.Execute(script, true);

        string[] templateNames = rawOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return templateNames.ToList();
    }

    public void Update(string vmName, int cpu, int ram)
    {
        Script script = Script.GetUpdateVmResourcesScript(vmName, cpu, ram);
        scriptService.Execute(script, false);
    }

    public void ResetPower(string vmName)
    {
        Script script = Script.GetResetVmPowerScript(vmName);
        scriptService.Execute(script, false);
    }

    public string GetVcenterInfo()
    {
        Script script = Script.GetVcenterInfoScript();
        return scriptService.Execute(script, true);
    }
}