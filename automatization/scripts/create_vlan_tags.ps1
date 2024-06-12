# Usage
if ($args.Length -ne 2) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_vlan_tags.ps1 '<username>__<password>__<esxi_host>' <vlan_tag>"
    Write-Host "Example:"
    Write-Host "./create_vlan_tags.ps1 'root__password__10.1.60.120' 60"
    exit
}

$esxi_host = $args[0] -split '__'
$vSwitchName = "vSwitch0"
$portgroupName = "VM Network"
$vlanId = $args[1]

Connect-VIServer -Server $esxi_host[2] -User $esxi_host[0] -Password $esxi_host[1] -Force
 
# Get the vSwitch and portgroup objects
$vSwitch = Get-VirtualSwitch -Name $vSwitchName
$portgroup = Get-VirtualPortGroup -VirtualSwitch $vSwitch -Name $portgroupName
 
# Update the VLAN ID for the portgroup
Set-VirtualPortGroup -VirtualPortGroup $portgroup -VlanId $vlanId -Confirm:$false
 
Disconnect-VIServer -Confirm:$false