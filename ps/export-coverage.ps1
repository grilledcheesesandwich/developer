param([string]$buildNumber = $(Read-Host -prompt "Build Number")) #prompt user for value if none provided)

$customVer = "OSPlatform_CoreOS.$($buildNumber)_USA_CEPC_WM_Premium_Min"
$path = "\\gadget\User\N-Z\unionp\wtt\coverage\$buildNumber"
#$path = "d:\coverage\$buildNumber"
if (!(test-path($path))) { throw "No coverage data for that build number" }
pushd $path
$sw = [Diagnostics.Stopwatch]::StartNew()
Write-Progress -Activity "Exporting Code Coverage data" -Status "Gathering covdata files"
$uniques = gci -r -filter *.covdata | group Name,Length
$i = 0
$uniques | % {
    $file = $_.Group[0].FullName
    Write-Progress -Activity "Exporting Code Coverage data" -Status "Exporting $file" -PercentComplete (++$i / $uniques.Count * 100)
    covdata /i $file /customver $customVer /db "server=cecodecover;database=Seven;uid=coverage;pwd=coverage" /ImportAndMerge
}
popd
"Exporting {0} unique covdata files took {1}" -f $uniques.Count, $sw.Elapsed