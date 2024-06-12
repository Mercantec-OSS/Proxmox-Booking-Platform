# Usage
if ($args.Length -ne 2) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_datacenter.ps1 '<username>__<password>__<vcenter>' <datacenter_name>"
    Write-Host "Example:"
    Write-Host "./create_datacenter.ps1 'admin@dc1.local__password__10.1.60.139' Datacenter01"
    exit
}

$vcenter = $args[0] -split '__'

Write-Host $vcenter[0]

Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

New-Datacenter -Name $args[1] -Location (Get-Folder -NoRecursion) -Confirm:$false

Disconnect-VIServer -Confirm:$false