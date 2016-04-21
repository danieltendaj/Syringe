$ErrorActionPreference = 'Stop';

$packageName = "Syringe"
$toolsDir = $(Split-Path -parent $MyInvocation.MyCommand.Definition)
. "$toolsDir\common.ps1"

if ((Test-IisInstalled) -eq $False)
{
    throw "IIS is not installed, please install it before continuing."
}

$version = "{{VERSION}}"
$url = "https://yetanotherchris.blob.core.windows.net/syringe/Syringe-$version.zip"
$url64 = $url

$packageArgs = @{
  packageName   = $packageName
  unzipLocation = $toolsDir
  fileType      = 'EXE' #only one of these: exe, msi, msu
  url           = $url
  url64bit      = $url64
}

# Download
Install-ChocolateyZipPackage $packageName $url $toolsDir

$serviceZip = "$toolsDir\Syringe.Service.$version.0.zip"
$websiteZip = "$toolsDir\Syringe.Web.$version.0.zip"

$serviceDir = "$toolsDir\Syringe.Service"
$websiteDir = "$toolsDir\Syringe.Web"

$serviceExe = "$toolsDir\Syringe.Service\Syringe.Service.exe"
$websiteSetupScript = "$toolsDir\Syringe.Web\bin\iis.ps1"

# Parse command line arguments - this function is required because of the context Chocolatey runs in, e.g.
# choco install syringe -packageParameters "/websitePort:82 /websiteDomain:'www.example.com' /restoreConfigs:true"
$arguments = @{}
$arguments["websitePort"] = 80;
$arguments["websiteDomain"] = "localhost";
$arguments["websiteDir"] = $websiteDir;
$arguments["restoreConfigs"] = "false";
Parse-Parameters($arguments);

# Backup the configs
cp "$serviceDir\configuration.json" "$serviceDir\configuration.bak.json" -Force -ErrorAction Ignore
cp "$serviceDir\environments.json" "$serviceDir\environments.bak.json" -Force -ErrorAction Ignore
cp "$websiteDir\web.config" "$serviceDir\web.bak.config" -Force -ErrorAction Ignore

# Unzip the service + website (overwrites existing files when upgrading)
Get-ChocolateyUnzip  $serviceZip $serviceDir "" $packageName
Get-ChocolateyUnzip  $websiteZip $websiteDir "" $packageName

# Restore the configs if it's set
if ($arguments["restoreConfigs"] -eq "true")
{
    cp "$serviceDir\configuration.bak.json" "$serviceDir\configuration.json" -Force -ErrorAction Ignore
    cp "$serviceDir\environments.bak.json" "$serviceDir\environments.json" -Force -ErrorAction Ignore
    cp "$serviceDir\web.bak.config" "$websiteDir\web.config" -Force -ErrorAction Ignore
}

# Uninstall the service if it exists
if (test-path $serviceExe)
{
	Write-Host "Service found - uninstalling the service."
    & sc.exe stop syringe 2>&1
	& $serviceExe uninstall
}

# Install the service
Write-Host "Installing the Syringe service." -ForegroundColor Green
& $serviceExe install --autostart --localsystem start

# Run the website installer
Write-Host "Setting up IIS site." -ForegroundColor Green
Invoke-Expression "$websiteSetupScript -websitePath $($arguments.websiteDir) -websiteDomain $($arguments.websiteDomain) -websitePort $($arguments.websitePort)"

# Info
Write-Host ""
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow
Write-Host "Setup complete." -ForegroundColor Green
Write-host "- MVC site          : http://$($arguments.websiteDomain):$($arguments.websitePort)/"
Write-Host "- REST api          : http://localhost:1981/swagger/"
Write-Host ""
Write-Host "Remember to remove the default website in IIS, if you are using port 80."
Write-host "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~" -ForegroundColor DarkYellow