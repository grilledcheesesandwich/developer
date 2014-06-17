param([string]$BuildDropSkuRoot = "\\build\Release\Apollo\WP8_OSPlat_BaseOS\WP8_OSPLAT_BASEOS.8157.9575.20111206-1815\MC.armfre")

#Write-Host -ForegroundColor Green "Enter mass storage mode (hold down vol-up/vol-down and reset) and press any key to continue..."
#$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
#\\winphonelabs\securestorage\Apollo\Project\OSPlart\BaseOS\ffutool\ffusettings.cmd g:

Write-Host -ForegroundColor Green 'Reset device, wait till you see "Entering flashing mode" and press any key to continue...'
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
$ffu = $BuildDropSkuRoot + "\Images\BaseSku\USA\Flash.ffu"
#$ffu = "$env:BINARY_ROOT\release\NOPLAT_ARMfre\flash.ffu"
\\winphonelabs\securestorage\Apollo\Project\OSPlat\BaseOS\ffutool\ffutool.exe -flash $ffu

Write-Host -ForegroundColor Green 'Enter mass storage mode and press any key to continue...'
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
\\winphonelabs\securestorage\Apollo\Project\OSPlat\DNS\FlashingTool\FlashPod.bat /n

Write-Host -ForegroundColor Green 'Reset device, wait until OS is booted and press any key to continue...'
$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
open-device -ip 10.178.205.207
$source = $BuildDropSkuRoot + "\bin\common\windowsphone\blockperflog.*"
putd $source c:\test\tux
