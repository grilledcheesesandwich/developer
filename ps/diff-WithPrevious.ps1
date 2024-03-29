param([string]$change, [string]$previousDpkPath)

# If user didn't supply a changelist #
if ([String]::IsNullOrEmpty($change))
{
    cls
    # Get all opened changelists
    $client = sd info | select-string "(?<=Client name: )\S+" | % { $_.Matches[0].Value }
    $changes = sd changes -r -s pending -c $client | select-string "(\d+)[^']*'([^']*)"

    $idx = 1
    $changelists = New-Object System.Collections.Generic.List[System.String]
    # Display all opened changelists
    $changes | % {
        $cl = $_.Matches[0].Groups[1].Value;
        $changelists.Add($cl);
        "" + $idx++ + "  " + $cl + "  " + $_.Matches[0].Groups[2].Value
    }

    # Get changelist # from user
    $change = read-host "`nPick a changelist"
    $change = $changelists[$change - 1]
}

# Display diff with latest jjpack on server
if ([String]::IsNullOrEmpty($previousDpkPath))
{
    $latestPack = gci \\winphonelabs\securestorage\Apollo\Project\OSPlat\BaseOS\dpk\$env:USERNAME\*.dpk | sort -prop LastWritetime | select -last 1
    if ($null -eq $latestPack) { Write-Error "No packs found on server"; return }
    $previousDpkPath = $latestPack.FullName
}
$tempPack = "$env:temp\$change.dpk"
sdpack pack $tempPack -c $change
set SDPDIFF=windiff.exe
sdpack diff2 $previousDpkPath $tempPack
del $tempPack
