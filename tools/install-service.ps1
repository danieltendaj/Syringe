#Requires -RunAsAdministrator

param(
	[switch]$uninstall
)

$services = @(
				"$PSScriptRoot\Syringe.Service.exe",
				"$PSScriptRoot\..\src\Syringe.Service\bin\Debug\Syringe.Service.exe",
				"$PSScriptRoot\..\src\Syringe.Service\bin\Release\Syringe.Service.exe"
			)

$servicePath = ""
foreach ($service in $services) 
{
	if(Test-Path $service)
	{
		$servicePath = $service
		Write-Host "Found service in $servicePath"
		break
	}	
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
