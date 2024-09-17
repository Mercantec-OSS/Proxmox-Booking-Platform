# Usage
if ($args.Length -ne 2) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_datacenter.ps1 '<username>__<password>__<vcenter>' <datacenter_name>"
    exit
}

$vcenter = $args[0] -split '__'

Write-Host $vcenter[0]

Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

New-Datacenter -Name $args[1] -Location (Get-Folder -NoRecursion) -Confirm:$false

Disconnect-VIServer -Confirm:$false