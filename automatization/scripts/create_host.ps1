# Usage
if ($args.Length -ne 3) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./create_host.ps1 '<username>__<password>__<vcenter>' '<username>__<password>__<esxi_host>' <cluster_name>"
    exit
}

$vcenter = $args[0] -split '__'
$new_host = $args[1] -split '__'

# create credential object
$host_ip = $new_host[2]
$username = $new_host[0]
$password = $new_host[1]
$secure_string_pwd = $password | ConvertTo-SecureString -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential -ArgumentList $username, $secure_string_pwd

Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force
 
# Get the cluster object
$cluster = Get-Cluster -Name $args[2]

# Suppress SSL certificate verification prompt and add the host to the cluster
$params = @{
    Name        = $host_ip
    Location    = $cluster
    Credential  = $credential
    Force       = $true
    ErrorAction = 'Stop'
}

# Add the host to the cluster
Add-VMHost @params

# Disconnect from the vCenter server
Disconnect-VIServer -Confirm:$false