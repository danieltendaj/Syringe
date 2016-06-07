$debugPath = "$PSScriptRoot\..\src\Syringe.Service\bin\Debug\Syringe.Service.exe"
$releasePath = "$PSScriptRoot\..\src\Syringe.Service\bin\Release\Syringe.Service.exe"

if(Test-Path $debugPath)
{
		Write-Host "Running DEBUG" -foreground red
		& $debugPath 
}
elseif(Test-Path $releasePath)
{
		Write-Host "Running RELEASE" -foreground red
		& $releasePath 
}
else
{
		throw "Error, unable to find both debug and release service binaries"
}