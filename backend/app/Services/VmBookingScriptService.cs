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

    public List<Iso> GetIsoList()
    {
        List<Iso> isoList = new List<Iso>();

        Script script = Script.GetIsoListScript();
        string output = scriptService.Execute(script, true);

        string[] lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        return lines.ToList().ConvertAll(Iso.GetFromScriptLine);
    }

    public void AttachIso(string vmName, string isoName)
    {
        Script script = Script.GetAttachIsoScript(vmName, isoName);
        scriptService.Execute(script, false);
    }

    public void DetachIso(string vmName)
    {
        Script script = Script.GetDetachIsoScript(vmName);
        scriptService.Execute(script, false);
    }

    public void AttachStorage(string vmName, int amountGb)
    {
        Script script = Script.GetAttachStorageScript(vmName, amountGb);
        scriptService.Execute(script, false);
    }
}