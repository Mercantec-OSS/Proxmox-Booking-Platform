# Usage
if ($args.Length -ne 2) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_windows_2022_vm.ps1 '<username>__<password>__<vcenter>' <target_host_name>"
    Write-Host "Example:"
    Write-Host "./create_windows_2022_vm.ps1 'root__password__10.1.60.120' 10.1.60.100"
    exit
}

$esxi_host = $args[0] -split '__'

# Connect to the vCenter server
Connect-VIServer -Server $esxi_host[2] -User $esxi_host[0] -Password $esxi_host[1] -Force

$vm_name_prefix = "Windows_2022_VM"
$cpu_cores = "2"
$ram_size = "4"
$storage_size = "90"
$storage_type = "Thin"
$target_host_name = $args[1]
$network_name = "VM Network"
$data_storage = "Shared_Storage_02_R1"
$guest_id = "windows2019srvNext_64Guest"
$iso_path = "[Shared_Storage_02_R1]\iso/SERVER_EVAL_x64FRE_en-us.iso"

# Find next VM number
$used_vms = @()
foreach ($vm in Get-VM) {
    if ($vm -match $vm_name_prefix) {
        $vm_name_tokens = $vm -split '-'
        $used_vms += [int]::Parse($vm_name_tokens[1])
    }
}
$next_vm_number = ($used_vms | Measure-Object -Maximum).Maximum + 1

$configs = @{
    Name               = "$vm_name_prefix-$next_vm_number"
    NumCpu             = $cpu_cores
    MemoryGB           = $ram_size
    DiskGB             = $storage_size
    DiskStorageFormat  = $storage_type
    Datastore          = $data_storage
    VMHost             = $target_host_name
    NetworkName        = $network_name
    GuestId            = $guest_id
}

#create VM
$created_vm = New-VM @configs

New-CDDrive -VM $created_vm -ISOPath $iso_path -StartConnected
Start-VM -VM $created_vm