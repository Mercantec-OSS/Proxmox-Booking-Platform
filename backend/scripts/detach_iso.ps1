if ($args.Length -ne 2) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./get_templates.ps1 '<username>__<password>__<vcenter>' <vm_name>"
    Write-Host "Example:"
    Write-Host "./get_templates.ps1 'admin@dc1.local__password__10.1.60.139'"
    exit
}

$vcenter = $args[0] -split '__'

# Connect to the vCenter server
$null = Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

# get the vm
$vmName = $args[1]
$vm = Get-VM -Name $vmName

# power off the VM
Stop-VM -VM $vm -Confirm:$false

# detach the iso
$cdDrive = Get-CDDrive -VM $vm
Set-CDDrive -CD $cdDrive -NoMedia:$true -Confirm:$false

# start vm
Start-VM -VM $vm -Confirm:$false 


