# Add the user
$computer = [ADSI]"WinNT://$Env:COMPUTERNAME,Computer"
$syringeUser = $computer.Create("User", "SyringeUser")
$syringeUser.SetPassword("Password")
$syringeUser.SetInfo()

$syringeUser.UserFlags = 64 + 65536
$syringeUser.SetInfo()

$syringeUser.FullName = "Syringe service user"
$syringeUser.SetInfo()

# Add to the Users group
$group = [ADSI]"WinNT://./Users,group"
$group.Add("WinNT://SyringeUser, user")

# Give the user permission to have HTTP listen on port 1981 (for self-hosting)
netsh http add urlacl url=http://*:1981/ user=SyringeUser

# Testing the service:
#runas /user:syringeuser "powershell.exe c:\code\syringe\tools\start-service.ps1"