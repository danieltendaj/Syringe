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
	& $servicePath 
}
else
{
	throw "Error, unable to find both debug and release service binaries"
}