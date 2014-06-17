# Get all opened changelists
$client = sd info | select-string "(?<=Client name: )\S+" | % { $_.Matches[0].Value }
$changes = sd changes -r -s pending -c $client | ? { $_ -match "(\d+)[^']*'([^']*)" } | % { 
    1 | Select-Object @{
        Name='Number'
        Expression={$matches[1]}
    }, @{
        Name='Description'
        Expression={$matches[2]}
    }
}

$changes = @($changes)

cls

# Display all opened changelists
for ($i = 1; $i -le $changes.Length; $i++)
{
    "{0}) {1} {2}" -f $i, $changes[$i - 1].Number, $changes[$i - 1].Description
}

# Get changelist # from user
$change = read-host "`nPick a changelist"
$change = $changes[$change - 1].Number

cls

# Get to/from branches
$branches = Get-ChildItem \ | where { $_.PSIsContainer } | where { $_.GetFiles("sd.ini").Length -ne 0 }
for ($i = 1; $i -le $branches.Length; $i++)
{
    "{0}) {1}" -f $i, $branches[$i - 1].FullName
}

$to = Read-Host "`nMigrate TO which branch?"
$to = $branches[$to - 1].FullName

# pack changelist
$target = "$env:TEMP\$change.cmd"
bbpack -c $change -f -o $target

# get branch path for FROM branch
$targetDir = (new-object System.IO.DirectoryInfo $pwd).Name
(sd files "$pwd\dirs") -match ".*(?=/$targetDir/dirs)"
$fromPath = $matches[0]

# get branch path for TO branch
$targetDir = "$pwd".Replace($env:_WINPHONEROOT, $to)
pushd $targetDir
$targetDir = (new-object System.IO.DirectoryInfo $targetDir).Name
(sd files dirs) -match ".*(?=/$targetDir/dirs)"
$toPath = $matches[0]

# get changelist description
$desc = sd describe -D $change
$desc | &"clip.exe"

# create changelist
$changeNumber = sd change -C "$desc"
$changeNumber -match "\d+"
$changeNumber = $matches[0]

&$target -m $fromPath $toPath -u -c $changeNumber
popd