if ($args.Length -ne 6) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_vm_from_template.ps1 '<username>__<password>__<vcenter>__<clustername>__<datastorename>' '<vm_template>' '<vm_name>' '<vm_root_password>' '<vm_user>' '<vm_password>'"
    exit
}

$vcenter_info = $args[0] -split '__'

# Connect to the vCenter server
Connect-VIServer -Server $vcenter_info[2] -User $vcenter_info[0] -Password $vcenter_info[1] -Force

# Define parameters
$cluster = $vcenter_info[3]
$datastore = $vcenter_info[4]
$vm_template = $args[1]
$vm_name = $args[2]
$vm_root_password = $args[3]
$vm_user = $args[4]
$vm_password = $args[5]

# Get the cluster object
$ClusterObj = Get-Cluster -Name $cluster

# Create the VM from template on the specific cluster
$vm = New-VM -Name $vm_name -Template $vm_template -ResourcePool $ClusterObj -Datastore $datastore

# Start the VM
Start-VM -VM $vm

if ($vm_template -notmatch "psw") {
    Write-Host "Password change is not required for this template. Exiting."
    exit
}

# Wait for VMware Tools to start
$maxIterations = 480
$delaySeconds = 15
for ($i = 1; $i -le $maxIterations; $i++) {
    $vm = Get-VM | Where-Object { $_.ExtensionData.Config.Name -eq $vm_name }
    $vmToolsStatus = $vm.ExtensionData.Guest.ToolsStatus
    $osType = $vm.Guest.OSFullName

    Write-Host "Iteration ${i}: VMware Tools status is '$vmToolsStatus', OS type is $osType"

    # await for host detection
    if ($osType -ne $null) {
        break
    }

    Start-Sleep -Seconds $delaySeconds
}

if ($osType -eq $null) {
    Write-Host "VMware Tools did not start after $maxIterations attempts. Exiting."
    exit
}

# Change password
Write-Host "VM with $vm"
Write-Host "OS type is $osType"

if ($osType -match "Linux") {
    $vm_admin_username = "root"
    $vmGuestCredential = New-Object System.Management.Automation.PSCredential ($vm_admin_username, (ConvertTo-SecureString $vm_root_password -AsPlainText -Force))
    
    $script = "echo '${vm_user}:${vm_password}' | chpasswd"
    Write-Host $script
    Invoke-VMScript -VM $vm -GuestCredential $vmGuestCredential -ScriptText $script -ErrorAction Stop
} 
elseif ($osType -match "Windows") {
    $vm_admin_username = $vm_user
    $vmGuestCredential = New-Object System.Management.Automation.PSCredential ($vm_admin_username, (ConvertTo-SecureString $vm_root_password -AsPlainText -Force))

    $script = "net user $vm_user $vm_password"
    Write-Host $script
    Invoke-VMScript -VM $vm -GuestCredential $vmGuestCredential -ScriptText $script -ErrorAction Stop
}