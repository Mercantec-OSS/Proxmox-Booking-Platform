Install-Module -Name VMware.PowerCLI -Force -AllowClobber
Set-PowerCLIConfiguration -Scope User -ParticipateInCEIP $false -Confirm:$false | Out-Null
