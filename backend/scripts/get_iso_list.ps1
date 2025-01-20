if ($args.Length -ne 3) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./get_templates.ps1 '<username>__<password>__<vcenter>' '<datacenter>' '<iso_library>'"
    Write-Host "Example:"
    Write-Host "./get_templates.ps1 'admin@dc1.local__password__10.1.60.139'"
    exit
}

$vcenter = $args[0] -split '__'
$datacenterName = $args[1]
$datastoreName = $args[2]

# Connect to the vCenter server
$null = Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

$datastore = Get-Datastore -Name $datastoreName
$isoList = Get-ChildItem -Path "vmstore:\$datacenterName\$($datastore.Name)" -Recurse | Where-Object { $_.Name -like "*.iso" }

# show all templates
foreach ($iso in $isoList) {
    $isoName = $iso.Name
    $isoPath = $iso.DatastoreFullPath
    Write-Host $isoName"||||"$isoPath
}

