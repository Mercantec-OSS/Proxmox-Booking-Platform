if ($args.Length -ne 3) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_windows_2022_vm.ps1 '<username>__<password>__<vcenter>' <template> <vm_name>"
    Write-Host "Example:"
    Write-Host "./create_windows_2022_vm.ps1 'root__password__10.1.60.120' 10.1.60.100"
    exit
}

$esxi_host = $args[0] -split '__'

# Connect to the vCenter server
Connect-VIServer -Server $esxi_host[2] -User $esxi_host[0] -Password $esxi_host[1] -Force

# Define parameters
$VMName = $args[2]
$Template = $args[1]
$Datastore = "DatastoreCluster"
$Cluster = "Cluster"

# Get the cluster object
$ClusterObj = Get-Cluster -Name $Cluster

# Create the VM from template on the specific cluster
$VM = New-VM -Name $VMName -Template $Template -ResourcePool $ClusterObj -Datastore $Datastore

# Start the VM
Start-VM -VM $VM

# Wait for the VM to start
Start-Sleep -Seconds 60

# Get the IP address of the VM using VMware Tools
$VM.Guest.IPAddress

# get vm by name
$vm = Get-VM -Name $VMName

