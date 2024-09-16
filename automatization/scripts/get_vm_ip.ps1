if ($args.Length -ne 2) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./get_vm_ip '<username>__<password>__<vcenter>' '<vm_name>'"
    exit
}

$esxi_host = $args[0] -split '__'
$null = Connect-VIServer -Server $esxi_host[2] -User $esxi_host[0] -Password $esxi_host[1] -Force

$VM = Get-VM -Name $args[1]
$VM.Guest.IPAddress
