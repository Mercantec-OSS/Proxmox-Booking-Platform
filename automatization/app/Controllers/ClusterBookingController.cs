namespace Controllers;

[ApiController]
[Route("cluster-booking")]
public class ClusterBookingController(CLusterBookingService clusterBookingService) : ControllerBase
{
    [HttpPost("shell-command")]
    public IActionResult ExecuteShellCommand(
        [FromQuery] string command,
        [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.ExecuteShellCommand(command, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("create-backup")]
    public IActionResult CreateBackup(
        [FromQuery] string hostUsername,
        [FromQuery] string hostPassword,
        [FromQuery] string hostIp,
        [FromQuery] string datastoreName,
        [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.CreateBackup(hostIp, hostUsername, hostPassword, datastoreName, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("create-cluster")]
    public IActionResult CreateCluster(
           [FromQuery] string vcenterUsername,
           [FromQuery] string vcenterPassword,
           [FromQuery] string vcenterIp,
           [FromQuery] string datacenterName,
           [FromQuery] string clusterName,
           [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.CreateCluster(vcenterIp, vcenterUsername, vcenterPassword, datacenterName, clusterName, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("create-datacenter")]
    public IActionResult CreateDatacenter(
        [FromQuery] string vcenterUsername,
        [FromQuery] string vcenterPassword,
        [FromQuery] string vcenterIp,
        [FromQuery] string datacenterName,
        [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.CreateDatacenter(vcenterIp, vcenterUsername, vcenterPassword, datacenterName, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("create-host")]
    public IActionResult CreateHost(
        [FromQuery] string vcenterUsername,
        [FromQuery] string vcenterPassword,
        [FromQuery] string vcenterIp,
        [FromQuery] string hostUsername,
        [FromQuery] string hostPassword,
        [FromQuery] string hostIp,
        [FromQuery] string clusterName,
        [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.CreateHost(vcenterIp, vcenterUsername, vcenterPassword, hostIp, hostUsername, hostPassword, clusterName, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("create-vlan")]
    public IActionResult CreateVlanTags(
        [FromQuery] string hostUsername,
        [FromQuery] string hostPassword,
        [FromQuery] string hostIp,
        [FromQuery] int vlan,
        [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.CreateVlan(hostUsername, hostPassword, hostIp, vlan, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("maintance-disable")]
    public IActionResult MaintanceDisable(
        [FromQuery] string hostUsername,
        [FromQuery] string hostPassword,
        [FromQuery] string hostIp,
        [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.MaintanceDisable(hostUsername, hostPassword, hostIp, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("maintance-enable")]
    public IActionResult MaintanceEnable(
       [FromQuery] string hostUsername,
       [FromQuery] string hostPassword,
       [FromQuery] string hostIp,
       [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.MaintanceEnable(hostUsername, hostPassword, hostIp, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("remove-vms")]
    public IActionResult RemoveVMS(
        [FromQuery] string hostUsername,
        [FromQuery] string hostPassword,
        [FromQuery] string hostIp,
        [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.RemoveVms(hostUsername, hostPassword, hostIp, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("reset-license")]
    public IActionResult ResetLicense(
        [FromQuery] string hostUsername,
        [FromQuery] string hostPassword,
        [FromQuery] string hostIp,
        [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.ResetLicense(hostUsername, hostPassword, hostIp, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("restore-backup")]
    public IActionResult RestoreBackup(
    [FromQuery] string hostUsername,
    [FromQuery] string hostPassword,
    [FromQuery] string hostIp,
    [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.RestoreBackup(hostIp, hostUsername, hostPassword, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("stop-and-remove-vms")]
    public IActionResult StopAndRemoveVMS(
    [FromQuery] string hostUsername,
    [FromQuery] string hostPassword,
    [FromQuery] string hostIp,
    [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.StopAndRemoveVms(hostUsername, hostPassword, hostIp, afterThan);
        return Ok(taskUuid);
    }

    [HttpPost("install-vcenter")]
    public IActionResult InstallVCenter(
    [FromBody] string base64JsonConfig,
    [FromQuery] string afterThan = "")
    {
        string taskUuid = clusterBookingService.InstallVCenter(base64JsonConfig, afterThan);
        return Ok(taskUuid);
    }
}
