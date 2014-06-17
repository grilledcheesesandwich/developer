param($releaseLabelLeft, $releaseLabelRight, $releaseLeft = "Blue", $releaseRight = "Blue")

function get-depotPath([string] $localPath, [string] $branch)
{
    $depotPath = $_.Substring($env:_WINPHONEROOT.Length)
    if ($depotPath -match "^\\src")
    {
        $depotPath = $depotPath -replace "^\\src", ""
    }
    else
    {
        $depotPath = "/root" + $depotPath
    }
    $depotPath = "//depot/$branch" + $depotPath
    $depotPath = $depotPath.Replace("\", "/")
    return $depotPath
}

$packages = cidir *.pkg.xml | ? { (gc $_) -match "(<Application)|(<Service)|(<Authorization)|(<Capabilities)" }
#$packages = findstr "<!--" .\Merged.Policy.xml | % { $_.Split("\")[-1].Split()[0] }

$packages += "$env:_WINPHONEROOT\tools\oak\misc\capabilitylist.cfg"

$leftDest = "$env:temp\diffs\$releaseLabelLeft"
$rightDest = "$env:temp\diffs\$releaseLabelRight"

$branchLeft = $releaseLabelLeft.Split(".")[0]
$branchRight = $releaseLabelRight.Split(".")[0]

md $leftDest
md $rightDest
(gc \\build\Release\$releaseLeft\$branchLeft\$releaseLabelLeft\MC.x86fre\Binaries\Images\vm_allres\Test\qps-ploc\Merged.Policy.xml) -replace "<!-- .*\\" > $leftDest\Merged.Policy.fixed.xml
(gc \\build\Release\$releaseRight\$branchRight\$releaseLabelRight\MC.x86fre\Binaries\Images\vm_allres\Test\qps-ploc\Merged.Policy.xml) -replace "<!-- .*\\" > $rightDest\Merged.Policy.fixed.xml

$packages | % {
    pushd (dir $_).DirectoryName
    $tokens = $_.Split("\")
    $normalizedPath = [String]::Join("\", $tokens[2..$tokens.Length])

    $leftPath = "$leftDest\$normalizedPath"
    $rightPath = "$rightDest\$normalizedPath"

    ni -Type File $leftPath -Force > $null
    ni -Type File $rightPath -Force > $null

    $depotPathLeft = get-depotPath $_ $branchLeft
    $depotPathRight = get-depotPath $_ $branchRight

    sd print "$depotPathLeft@$releaseLabelLeft" | select -Skip 1 > $leftPath
    sd print "$depotPathRight@$releaseLabelRight" | select -Skip 1 > $rightPath
}