if ($args.Length -ne 3) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./get_templates.ps1 '<username>__<password>__<vcenter>' <vm_name> <iso_path>"
    Write-Host "Example:"
    Write-Host "./get_templates.ps1 'admin@dc1.local__password__10.1.60.139'"
    exit
}

$vcenter = $args[0] -split '__'
$isoPath = $args[2]

# Connect to the vCenter server
$null = Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

# get the vm
$vmName = $args[1]
$vm = Get-VM -Name $vmName

# power off the VM
Stop-VM -VM $vm -Confirm:$false

# attach the iso
$cdDrive = Get-CDDrive -VM $vm
Set-CDDrive -CD $cdDrive -IsoPath "$isoPath" -StartConnected:$true -NoMedia:$false -Confirm:$false

# start vm
Start-VM -VM $vm -Confirm:$false 

