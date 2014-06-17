$winVerValue = regd query "HKLM\Software\Microsoft\Windows NT\CurrentVersion" /v BuildLabEx
$winVerValue
$winver = ($winVerValue | select-string "\b(\d+)\..*").Matches.Groups[1].Value
$buildInfo = typed \windows\system32\buildinfo.xml
$bi = ([xml]$buildInfo)."build-information"
$sku = "{0}.{1}{2}" -f $bi."target-os", $bi."target-cpu", $bi."build-type"
"{0}.$winver.{1}.{2}\$sku`n" -f $bi."release-label", $bi."parent-branch-build", $bi."build-time"
