# Usage
if ($args.Length -ne 3) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_cluster.ps1 '<username>__<password>__<vcenter>' <datacenter_name> <cluster_name>"
    exit
}

$vcenter = $args[0] -split '__'

Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

New-Cluster -Name $args[2] -Location $args[1] -Confirm:$false

Disconnect-VIServer -Confirm:$false