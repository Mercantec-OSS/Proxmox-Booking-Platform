if ($args.Length -ne 3) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./get_templates.ps1 '<username>__<password>__<vcenter>' <vm_name> <amountGb>"
    Write-Host "Example:"
    Write-Host "./get_templates.ps1 'admin@dc1.local__password__10.1.60.139'"
    exit
}

$vcenter = $args[0] -split '__'
$amountGb = $args[2]

# Connect to the vCenter server
$null = Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

# get the vm
$vmName = $args[1]
$vm = Get-VM -Name $vmName

# get the number of disks attached to the vm
$currentDisk = (Get-HardDisk -VM $vm).Count

# check if the vm already has 3 or more disks attached
if ($currentDisk -ge 3) {
    Write-Host "VM already has 3 disks attached."
    exit
}

# Attach storage to the VM
New-HardDisk -VM $vm -CapacityGB $amountGb -StorageFormat Thin
