namespace Services;

public class VmBookingService(ScriptFactory scriptFactory)
{

    public void Create(string name, string template, string vmRootPassword, string vmUser, string vmPassword)
    {
        ICommand command = scriptFactory.GetCreateVmScript(name, template, vmRootPassword, vmUser, vmPassword);
        command.Execute(false);
    }

    public void Remove(string name)
    {
        ICommand command = scriptFactory.GetRemoveVmScript(name);
        command.Execute(false);
    }

    public List<string> GetTemplates()
    {
        ICommand command = scriptFactory.GetTemlatesScript();
        string rawOutput = command.Execute(true);

        string[] templateNames = rawOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return templateNames.ToList();
    }

    public void Update(string vmName, int cpu, int ram)
    {
        ICommand command = scriptFactory.GetUpdateVmResourcesScript(vmName, cpu, ram);
        command.Execute(false);
    }

    public void ResetPower(string vmName)
    {
        ICommand command = scriptFactory.GetResetVmPowerScript(vmName);
        command.Execute(false);
    }

    public string GetVcenterInfo()
    {
        ICommand command = scriptFactory.GetVcenterInfoScript();
        return command.Execute(true);
    }
}