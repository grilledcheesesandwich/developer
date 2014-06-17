param([string]$name)
if (![String]::IsNullOrEmpty($name))
{
    $newfile = "$env:_DEVDIR\ps\$name.ps1"
    new-item $newfile -type file
    sd add -t text $newfile
    ise $newfile
}