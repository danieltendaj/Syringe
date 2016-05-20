function AddUser($username, $password)
{
    $computer = [ADSI]"WinNT://$Env:COMPUTERNAME,Computer"
    $existingUser = $computer.Children | where {$_.SchemaClassName -eq 'user' -and $_.Path.Contains($username) }

    if (!$existingUser)
    {
        # Add the user
        $syringeUser = $computer.Create("User", $username)
        $syringeUser.SetPassword($password)
        $syringeUser.SetInfo()

        $syringeUser.UserFlags = 64 + 65536
        $syringeUser.SetInfo()

        $syringeUser.FullName = "Syringe service user"
        $syringeUser.SetInfo()

        # Add to the Users group
        $group = [ADSI]"WinNT://./Users,group"
        $group.Add("WinNT://$username, user")

        # Give the user permission to have HTTP listen on port 1981 (for self-hosting)
        netsh http add urlacl url=http://*:1981/ user="$username"

        # Testing the service:
        #runas /user:syringeuser "powershell.exe c:\code\syringe\tools\start-service.ps1"
    }
}