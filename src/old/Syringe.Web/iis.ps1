#=================================================================
# Add a Syringe app pool and website, removing any existing ones.
#=================================================================
param ([string] $websitePath = $PSScriptRoot, [string] $removeOnly = $false, $websiteDomain = "localhost", $websitePort = 80)

$appPoolName = "Syringe"
$websiteName = "Syringe"

Import-Module WebAdministration

function Test-WebAppPool($Name) {
    return Test-Path "IIS:\AppPools\$Name"
}

function Test-Website($Name) {
    return Test-Path "IIS:\Sites\$Name"
}

#=================================================================
# Remove existing app pool and site
#=================================================================
if (Test-WebAppPool $appPoolName)
{
    Write-Host "  Removing app pool $appPoolName"
    Remove-WebAppPool -Name $appPoolName -WarningAction Ignore
}

if (Test-Website $websiteName)
{
    Write-Host "  Removing website $websiteName"
    Remove-Website -Name $websiteName -WarningAction Ignore
}

if ($removeOnly -eq $true)
{
	exit;
}

#=================================================================
# Add the app pool first
#=================================================================
Write-Host "  Adding app pool $appPoolName (v4, localservice)"

New-WebAppPool -Name $appPoolName -Force | Out-Null
Set-ItemProperty "IIS:\AppPools\$appPoolName" managedRuntimeVersion v4.0
Set-ItemProperty "IIS:\AppPools\$appPoolName" managedPipelineMode Integrated
Set-ItemProperty "IIS:\AppPools\$appPoolName" processModel -value @{userName="";password="";identitytype=1}
Set-ItemProperty "IIS:\AppPools\$appPoolName" processModel.idleTimeout -value ([TimeSpan]::FromMinutes(0))
Set-ItemProperty "IIS:\AppPools\$appPoolName" processModel.pingingEnabled -value true #disable for debuging

#=================================================================
# Syringe website
#=================================================================
Write-Host "  Adding website $websiteName (id:$websitePort, port: $websitePort, path: $websitePath)"

New-Website -Name $websiteName -Id $websitePort -Port $websitePort -PhysicalPath $websitePath -ApplicationPool $appPoolName -Force  | Out-Null
New-WebBinding -Name $websiteName -IPAddress "*" -Port $websitePort -HostHeader $websiteDomain