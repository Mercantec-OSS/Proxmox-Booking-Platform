# Usage
if ($args.Length -ne 1) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./stop_vms.ps1 '<username>__<password>__<esxi_host>'"
    Write-Host "Example:"
    Write-Host "./stop_vms.ps1 'root__password__10.1.60.120'"
    exit
}

$esxi_host = $args[0] -split '__'
 
# Connect to the vCenter server
Connect-VIServer -Server $esxi_host[2] -User $esxi_host[0] -Password $esxi_host[1] -Force
 
# Get all virtual machines
$vms = Get-VM
 
# Stop each virtual machine
foreach ($vm in $vms) {
    Stop-VM -VM $vm -Confirm:$false
}
 
# Disconnect from the vCenter server
Disconnect-VIServer -Confirm:$false
