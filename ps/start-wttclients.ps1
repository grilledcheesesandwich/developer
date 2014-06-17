param($user = "redmond\wp8bsp", $password = "1qazxsw2!QAZXSW@")

$machines = gc .\machinelist.txt
$exeName = "WTTMobileClient"
$exePath = "C:\Program Files (x86)\WTTMobile\Client\WTTMobileClient.exe"

foreach ($machine in $machines)
{
    if (!(gps -ComputerName $machine -Name $exeName -ErrorAction SilentlyContinue))
    {
        $sessions = Invoke-Command -ComputerName $machine -ScriptBlock { qwinsta }
        $username = $user.Split("\") | select -Last 1
        $matches = $sessions | ss "$username.*(\d)"
        $sessionId = $matches.Matches.Groups[1].Value
        "psexec \\$machine -i $sessionId -d -u $user -p $password $exePath"
        psexec \\$machine -i $sessionId -d -u $user -p $password $exePath
    }
}