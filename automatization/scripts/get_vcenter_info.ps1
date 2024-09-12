if ($args.Length -ne 1) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./get_vcenter_info.ps1 '<username>__<password>__<vcenter>'"
    exit
}

$vcenter = $args[0] -split '__'
$null = Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

$VMs = Get-VM
$vmCount = $VMs.Count

$Templates = Get-Template
$TemplatesCount += $Templates.Count

$hosts = Get-VMHost

$cpuUsageGHz = ($hosts | Measure-Object -Property CpuUsageMhz -Sum).Sum / 1000 
$cpuUsageGHz = "{0:N1}" -f $cpuUsageGHz
$cpuTotalGHz = ($hosts | Measure-Object -Property CpuTotalMhz -Sum).Sum / 1000 
$cpuTotalGHz = "{0:N1}" -f $cpuTotalGHz

$ramUsedGB = ($hosts | Measure-Object -Property MemoryUsageGB -Sum).Sum
$ramUsedGB = "{0:N1}" -f $ramUsedGB

$ramTotalGB = ($hosts | Measure-Object -Property MemoryTotalGB -Sum).Sum
$ramTotalGB = "{0:N1}" -f $ramTotalGB

$datastores = Get-Datastore
$storageUsedGB = ($datastores | Measure-Object -Property CapacityGB -Sum).Sum - ($datastores | Measure-Object -Property FreeSpaceGB -Sum).Sum
$storageUsedGB = "{0:N1}" -f $storageUsedGB
$storageTotalGB = ($datastores | Measure-Object -Property CapacityGB -Sum).Sum
$storageTotalGB = "{0:N1}" -f $storageTotalGB

$totalHosts = $hosts.Count
$activeHosts = ($hosts | Where-Object { $_.ConnectionState -eq 'Connected' }).Count

Write-Host "total_hosts:$totalHosts"
Write-Host "active_hosts:$activeHosts"
Write-Host "cpu_total:$cpuTotalGHz GHz"
Write-Host "cpu_usage:$cpuUsageGHz GHz"
Write-Host "ram_total:$ramTotalGB GB"
Write-Host "ram_usage:$ramUsedGB GB"
Write-Host "storage_total:$storageTotalGB GB"
Write-Host "storage_usage:$storageUsedGB GB"
Write-Host "total_vms:$vmCount"
Write-Host "total_templates:$TemplatesCount"

Disconnect-VIServer -Confirm:$false