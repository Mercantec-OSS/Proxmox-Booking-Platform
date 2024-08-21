if ($args.Length -ne 2) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./reset_vm_ip '<username>__<password>__<vcenter>' '<vm_name>'"
    Write-Host "Example:"
    Write-Host "./reset_vm_ip 'admin@dc1.local__password__10.1.60.139' '43e3cb00-2d58-11ef-88af-07693f6764d2'"
    exit
}

$esxi_host = $args[0] -split '__'
Connect-VIServer -Server $esxi_host[2] -User $esxi_host[0] -Password $esxi_host[1] -Force

$VM = Get-VM -Name $args[1]
if ($VM.PowerState -eq "PoweredOff") {
    Write-Host "VM is powered off. Powering on..."
    Start-VM -VM $VM -Confirm:$false
} else {
    Write-Host "VM is powered on. Restarting..."
    Restart-VM -VM $VM -Confirm:$false
}