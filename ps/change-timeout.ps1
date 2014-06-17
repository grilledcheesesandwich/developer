param([switch]$Manual, [switch]$SetOnly)

if (!$DeviceAddress) { openphone }
if ($DeviceAddress)
{
    if (!$SetOnly)
    {
        if ($Manual)
        {
            #putd \\UNDEV\shared\temp\AntiLeakLock.exe c:\programs\antileaklock
            #execd sectool sac c:\programs\antileaklock\antileaklock.exe

            regd add HKEY_LOCAL_MACHINE\Software\Microsoft\AntiLeakLock /v MaxInactivityTime /t REG_DWORD /d 10
            execd c:\programs\antileaklock\antileaklock.exe
        }
        else
        {
            iutool -p \\UNDEV\shared\temp\Microsoft.Phone.Test.DevMgmt.Enterprise.AntiLeakLock.Customization.spkg
        }
    }

    regd add HKEY_LOCAL_MACHINE\software\microsoft\Shell\Timeouts /v DCUserIdle /t REG_DWORD /d 0xB4
    regd add HKEY_LOCAL_MACHINE\software\microsoft\Shell\Timeouts /v ACUserIdle /t REG_DWORD /d 0xB4
    regd add HKEY_LOCAL_MACHINE\software\microsoft\Shell\Timeouts /v PasswordTimeout /t REG_DWORD /d 0x258
}