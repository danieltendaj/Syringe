# ===============================================================================
#
# Syringe developer setup script.
#
# This script does the following:
# 1. Runs the IIS setup script
# 2. Creates C:\syringe\teamname (default path for tests cases)
# 3. Copies an example test case XML file to that location
# ===============================================================================
$ErrorActionPreference = "Stop"
$websiteDir = Resolve-Path ".\src\Syringe.Web\"
$serviceDir  = Resolve-Path ".\src\Syringe.Service\"

Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Syringe setup script. " -ForegroundColor DarkYellow
Write-Host "Please read the README file before running this script. " -ForegroundColor DarkYellow
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow

# Restore submodules
git submodule init
git submodule update

# Install nuget
Write-Host "Installing Nuget." -ForegroundColor Green
choco install nuget.commandline -y

# Install MongoDB
Write-Host "Installing MongoDB" -ForegroundColor Green
$mongoDataDir = $env:ChocolateyInstall +"\lib\mongodata"
$oldSysDrive = $env:systemdrive
$env:systemdrive = $mongoDataDir
choco install mongodb -version 3.0.3
$env:systemdrive = $oldSysDrive


if(!(Test-Path "$serviceDir\configuration.json"))
{
	Write-Host "Restoring config file from default..." -ForegroundColor Green
	Copy-Item "$serviceDir\configuration.default.json" "$serviceDir\configuration.json"
}

# Build
Write-Host "Building solution." -ForegroundColor Green
.\build\build.ps1 "Debug"

# Setup IIS
Write-Host "Running IIS install script." -ForegroundColor Green
.\src\Syringe.Web\bin\iis.ps1 -websitePath "$websiteDir" -websitePort 1980

# Done
Write-Host ""
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Setup complete." -ForegroundColor Green
Write-host "Now start the REST data service using tools\start-service.ps1" -ForegroundColor Cyan
Write-Host ""
Write-host "- MVC site          : http://localhost:1980/"
Write-Host "- REST api          : http://localhost:1981/swagger/"
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow