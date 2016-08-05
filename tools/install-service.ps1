#Requires -RunAsAdministrator

param(
	[switch]$uninstall
)

$servicePath = "$PSScriptRoot\..\src\Syringe.Service\bin\Debug\Syringe.Service.exe"

if(!(Test-Path $servicePath))
{
		Write-Host "Detected RELEASE binaries" -foreground red
		$servicePath = "$PSScriptRoot\..\src\Syringe.Service\bin\Release\Syringe.Service.exe"
}

if(Test-Path $servicePath)
{
		Write-Host "Uninstalling Syringe service..." -foreground green
		& $servicePath stop
		& $servicePath uninstall

		if($uninstall -eq $false)
		{
				Write-Host "Installing Syringe service..." -foreground green
				& $servicePath install
				& $servicePath start
		}

		Write-Host "All done :-)" -foreground green
}
else
{
		throw "Error, unable to find both debug and release service binaries"
}
