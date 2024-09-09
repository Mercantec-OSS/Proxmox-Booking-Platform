if ($args.Length -ne 3) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_vm_from_template.ps1 '<username>__<password>__<vcenter>__<clustername>__<datastorename>' <template> <vm_name>"
    Write-Host "Example:"
    Write-Host "./create_vm_from_template.ps1 'root__password__10.1.60.120__Cluster__DatastoreCluster' template vm"
    exit
}

$vcenter_info = $args[0] -split '__'

# Connect to the vCenter server
Connect-VIServer -Server $vcenter_info[2] -User $vcenter_info[0] -Password $vcenter_info[1] -Force

# Define parameters
$VMName = $args[2]
$Template = $args[1]
$Datastore = $vcenter_info[4]
$Cluster = $vcenter_info[3]

# Get the cluster object
$ClusterObj = Get-Cluster -Name $Cluster

# Create the VM from template on the specific cluster
$VM = New-VM -Name $VMName -Template $Template -ResourcePool $ClusterObj -Datastore $Datastore

# Start the VM
Start-VM -VM $VM