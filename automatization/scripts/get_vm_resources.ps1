# Check for the correct number of arguments
if ($args.Length -ne 2) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage: ./get_vm_info.ps1 '<username>__<password>__<vcenter>' '<vm_name>'"
    exit
}

# Extract input parameters
$vcenter = $args[0] -split '__'
$vm_name = $args[1]

# Connect to the vCenter server
$null = Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

# Get VM information by UUID
$vm = Get-VM | Where-Object { $_.ExtensionData.Config.Name -eq $vm_name }

# Output VM CPU and RAM info
if ($vm) {
    $name = $vm.Name
    $ip = $vm.Guest.IPAddress
    $cpu = $vm.NumCpu
    $ram = $vm.MemoryGB
    Write-Host "name: $name"
    Write-Host "ip: $ip"
    Write-Host "cpu: $cpu"
    Write-Host "ram: $ram"
} else {
    Write-Host "VM with UUID $vm_name not found."
}

# Disconnect from the vCenter server
Disconnect-VIServer -Confirm:$false