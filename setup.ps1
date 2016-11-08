#Requires -RunAsAdministrator

# ===============================================================================
#
# Syringe developer setup script.
#
# This script does the following:
# 1. Runs the IIS setup script
# 2. Installs Mongo & NodeJS
# 3. Builds solution
# 4. Creates default configuration
# ===============================================================================
$ErrorActionPreference = "Stop"
$websiteDir = Resolve-Path ".\src\Syringe.Web\"
$serviceDir  = Resolve-Path ".\src\Syringe.Service\"

Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Syringe setup script. " -ForegroundColor DarkYellow
Write-Host "For troubleshooting please read the README file. " -ForegroundColor DarkYellow
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow

# Restore submodules
git submodule init
git submodule update

# Install nuget
Write-Host "Installing Nuget." -ForegroundColor Green
choco install nuget.commandline -y

# Install Mongo
Write-Host "Installing MongoDB" -ForegroundColor Green
$mongoDataDir = $env:ChocolateyInstall +"\lib\mongodata"
$oldSysDrive = $env:systemdrive
$env:systemdrive = $mongoDataDir
choco install mongodb -version 3.0.3
$env:systemdrive = $oldSysDrive

# NodeJS is needed for Gulp
Write-Host "Installing NodeJS/Gulp"-ForegroundColor Cyan
choco install nodejs -y

# Refresh the path vars for npm (Chocolatey 0.98+)
refreshenv

# try and stop the service if it's installed
$servicePath = ".\src\Syringe.Service\bin\release\Syringe.Service.exe"
if(Test-Path $servicePath)
{
    & $servicePath stop
}

try
{
    pushd src\Syringe.Web
    npm install
    npm install gulp -g

    # Refresh the path vars for Gulp
    refreshenv

    gulp -b ".\" --gulpfile "gulpfile.js" default
}
finally
{
    popd
}

# Build the csproj
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
