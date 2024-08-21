if ($args.Length -ne 1) {
    Write-Host "Incorrect number of arguments."
    Write-Host "Usage:"
    Write-Host "./get_templates.ps1 '<username>__<password>__<vcenter>'"
    Write-Host "Example:"
    Write-Host "./get_templates.ps1 'admin@dc1.local__password__10.1.60.139'"
    exit
}

$vcenter = $args[0] -split '__'

# Connect to the vCenter server
$null = Connect-VIServer -Server $vcenter[2] -User $vcenter[0] -Password $vcenter[1] -Force

# Retrieve the list of VM templates
$templates = Get-Template

# Filter the templates to include only those with "template" in the name (case-insensitive)
$filteredTemplates = $templates | Where-Object { $_.Name -match 'template' }

# Return only the list of names
$filteredTemplates.Name