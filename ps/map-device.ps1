param([Parameter(Position=0, ParameterSetName='MAC')] [string]$MacAddress, [Parameter(ParameterSetName='IP')] [string]$IPAddress)  

function GetIPFromMac([string]$MacAddress)
{
    $formattedMac = ([Regex]::Matches($MacAddress, '\w\w') | % { $_.Value }) -Join '-'
    (-split ((arp -a) -match $formattedMac))[0]
}

net use v: /delete 2>&1 > $null

if (!$IPAddress)
{
    "No IP address specified, looking up by MAC"
    $IPAddress = GetIPFromMac $MacAddress
    if (!$IPAddress)
    {
        Write-Host -ForegroundColor Red "No IP found for $MacAddress, is device booted?"
        break
    }
    "Found IP: $IPAddress"
}

$VBScript = @"
    set oShell = CreateObject("WScript.Shell")
    oShell.run("Telnet $IPAddress")

    WScript.Sleep 500
    oShell.SendKeys("net.exe user user password /add {Enter}")

    WScript.Sleep 500
    oShell.SendKeys("net.exe localgroup Administrators user /add {Enter}")

    WScript.Sleep 500
    oShell.SendKeys("exit{Enter}")
    WScript.Sleep 500
    oShell.SendKeys("{Enter}")
"@;

$VBScript > $env:temp\telnetAddUser.vbs

"Launching telnet and configuring device"
& $env:temp\telnetAddUser.vbs

sleep 5

net use v: \\$IPAddress\c$ password /u:user > $null
if ($LASTEXITCODE -eq 0)
{
    Write-Host -ForegroundColor Green "Mapped remote device to V:\"
}
else
{
    Write-Host -ForegroundColor Red "Failed to map remote device to V:"
}