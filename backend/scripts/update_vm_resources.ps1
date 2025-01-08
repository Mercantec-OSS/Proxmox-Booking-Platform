# Check for the correct number of arguments
if ($args.Length -ne 4) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage: ./update_vm_resources.ps1 '<username>__<password>__<vcenter>' '<vm_name>' <cpu_count> <ram_gb>"
    exit
}

# Extract input parameters
$vcenter = $args[0] -split '__'
$vm_name = $args[1]
$cpu_count = [int]$args[2]
$ram_gb = [int]$args[3]

# Connect to the vCenter server
$null = Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

# Get VM by UUID
$vm = Get-VM | Where-Object { $_.ExtensionData.Config.Name -eq $vm_name }

if ($vm) {
    $null = Stop-VM -VM $vm -Confirm:$false
    $null = Set-VM -VM $vm -NumCpu $cpu_count -MemoryGB $ram_gb -Confirm:$false
    $null = Start-VM -VM $vm -Confirm:$false

    Write-Host "Successfully updated VM: $($vm.Name)"
    Write-Host "New CPU Cores: $cpu_count"
    Write-Host "New RAM Allocated: $ram_gb GB"
} else {
    Write-Host "VM with UUID $vm_name not found."
}

# Disconnect from the vCenter server
Disconnect-VIServer -Confirm:$false