namespace Services;

public class CLusterBookingService(ScriptFactory scriptFactory)
{
    private string CreateTask(ICommand command, string afterThan)
    {
        var task = new Models.Task();
        task.Command = command;
        task.AfterThan = afterThan;
        TaskBackgoundService.AddTask(task);
        return task.Uuid;
    }

    public string ExecuteShellCommand(string command, string afterThan)
    {
        ICommand cmd = new ShellCommand(command);
        return CreateTask(cmd, afterThan);
    }

    public string CreateBackup(string hostIp, string hostUsername, string hostPassword, string datastoreName, string afterThan)
    {
        ICommand command = scriptFactory.GetCreateBackupScript(hostIp, hostUsername, hostPassword, datastoreName);
        return CreateTask(command, afterThan);
    }

    public string CreateCluster(string vcenterIp, string vcenterUsername, string vcenterPassword, string datacenterName, string clusterName, string afterThan)
    {
        ICommand command = scriptFactory.GetCreateClusterScript(vcenterIp, vcenterUsername, vcenterPassword, datacenterName, clusterName);
        return CreateTask(command, afterThan);
    }

    public string CreateDatacenter(string vcenterIp, string vcenterUsername, string vcenterPassword, string datacenterName, string afterThan)
    {
        ICommand command = scriptFactory.GetCreateDatacenterScript(vcenterIp, vcenterUsername, vcenterPassword, datacenterName);
        return CreateTask(command, afterThan);
    }

    public string CreateHost(string vcenterIp, string vcenterUsername, string vcenterPassword, string hostIp, string hostUsername, string hostPassword, string clusterName, string afterThan)
    {
        ICommand command = scriptFactory.GetCreateHostScript(vcenterIp, vcenterUsername, vcenterPassword, hostIp, hostUsername, hostPassword, clusterName);
        return CreateTask(command, afterThan);
    }

    public string CreateVlan(string hostUsername, string hostPassword, string hostIp, int vlanNumber, string afterThan)
    {
        ICommand command = scriptFactory.GetCreateVlanScript(hostUsername, hostPassword, hostIp, vlanNumber);
        return CreateTask(command, afterThan);
    }

    public string MaintanceDisable(string hostUsername, string hostPassword, string hostIp, string afterThan)
    {
        ICommand command = scriptFactory.GetMaintanceDisableScript(hostUsername, hostPassword, hostIp);
        return CreateTask(command, afterThan);
    }

    public string MaintanceEnable(string hostUsername, string hostPassword, string hostIp, string afterThan)
    {
        ICommand command = scriptFactory.GetMaintanceEnableScript(hostUsername, hostPassword, hostIp);
        return CreateTask(command, afterThan);
    }

    public string RemoveVms(string hostUsername, string hostPassword, string hostIp, string afterThan)
    {
        ICommand command = scriptFactory.GetRemoveVmsScript(hostIp, hostUsername, hostPassword);
        return CreateTask(command, afterThan);
    }

    public string ResetLicense(string hostUsername, string hostPassword, string hostIp, string afterThan)
    {
        ICommand command = scriptFactory.GetResetLicenseScript(hostIp, hostUsername, hostPassword);
        return CreateTask(command, afterThan);
    }

    public string RestoreBackup(string hostIp, string hostUsername, string hostPassword, string afterThan)
    {
        ICommand command = scriptFactory.GetRestoreBackupScript(hostIp, hostUsername, hostPassword);
        return CreateTask(command, afterThan);
    }

    public string StopAndRemoveVms(string hostUsername, string hostPassword, string hostIp, string afterThan)
    {
        ICommand command = scriptFactory.GetStopAndRemoveVmsScript(hostIp, hostUsername, hostPassword);
        return CreateTask(command, afterThan);
    }

    public string InstallVCenter(string base64JsonConfig, string afterThan)
    {
        // write json config to file in temp folder
        string jsonConfig = Encoding.UTF8.GetString(Convert.FromBase64String(base64JsonConfig));
        string filePath = FileWriter.WriteToFile(jsonConfig);

        // create task to install vcenter
        ICommand command = scriptFactory.GetInstallVcenterScript(filePath);
        string taskUuid = CreateTask(command, afterThan);

        // task for remove json config file after install
        ICommand removeConfigFileCommand = scriptFactory.GetRemoveJsonConfigScript(filePath);
        CreateTask(removeConfigFileCommand, taskUuid);

        return taskUuid;
    }
}