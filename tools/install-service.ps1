$debugPath = "$PSScriptRoot\..\src\Syringe.Service\bin\Debug\Syringe.Service.exe"
$releasePath = "$PSScriptRoot\..\src\Syringe.Service\bin\Release\Syringe.Service.exe"

if(Test-Path $debugPath)
{
		Write-Host "Running DEBUG" -foreground red
		& $debugPath stop
		& $debugPath uninstall
		& $debugPath install
		& $debugPath start
}
elseif(Test-Path $releasePath)
{
		Write-Host "Running RELEASE" -foreground red
		& $releasePath stop
		& $releasePath uninstall
		& $releasePath install
		& $releasePath start
}
else
{
		throw "Error, unable to find both debug and release service binaries"
}