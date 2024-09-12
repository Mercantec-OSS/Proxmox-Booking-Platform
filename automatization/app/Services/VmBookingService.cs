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

    public VmDTO Get(string vmName)
    {
        ICommand command = scriptFactory.GetVmScript(vmName);
        string rawOutput = command.Execute(true);

        string[] outputLines = rawOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        string name = "";
        string ip = "";
        int cpuCores = 0;
        int ramGb = 0;

        // parse data from output
        foreach (var line in outputLines)
        {
            if (line.StartsWith("name:"))
            {
                name = line.Split(':')[1].Trim();
            }
            else if (line.StartsWith("ip:"))
            {
                ip = line.Split(':')[1].Trim().Split(" ")[0].Trim();
            }
            else if (line.StartsWith("cpu:"))
            {
                cpuCores = int.Parse(line.Split(':')[1].Trim());
            }
            else if (line.StartsWith("ram:"))
            {
                ramGb = int.Parse(line.Split(':')[1].Trim());
            }
        }

        return new VmDTO()
        {
            Name = name,
            Ip = ip,
            Cpu = cpuCores,
            Ram = ramGb
        };
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