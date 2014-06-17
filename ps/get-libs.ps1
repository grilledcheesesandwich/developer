param([string] $deptree = "common",
      [string] $subdir,
      [string] $type = "lib")

function Extract-Zip
{
	param([string]$zipfilename, [string] $destination)

	if(test-path($zipfilename))
	{	
        if (!(test-path $destination)) { $null= md $destination }
    
		$shellApplication = new-object -com shell.application
		$zipPackage = $shellApplication.NameSpace($zipfilename)
		$destinationFolder = $shellApplication.NameSpace($destination)
		$destinationFolder.CopyHere($zipPackage.Items())
	}
}

function Extract-Zips-Under-Path([string] $root, [string] $path)
{
    $count = 0
    $zips = dir -Path $path -Filter *.zip -Recurse
    if ($type -eq "inc")
    {
        $zips = $zips | where { $_.Directory.FullName.EndsWith("$type") }
    }
    else
    {
        $zips = $zips | where { $_.Directory.FullName.EndsWith("$type\$env:_TGTCPU\$env:WINCEDEBUG") }
    }
    $zips = @($zips)
    $zips | % {
        $dir = $_.Directory.FullName
        $destPath = $dir.Replace($root, $env:_WINCEROOT)
        Write-Progress -Activity "Enumerating and expanding binsync zips" -Status $destpath -PercentComplete (++$count / $zips.Count * 100)
        extract-zip $_.FullName $destPath
    }
}

# Get all drop points, sort by date
$buildVer = gc $env:_PUBLICROOT\smartfon\buildver.bat
$branch = $buildVer | select-string "WM7_\w+$" | % { $_.Matches[0].Value }
if ($branch -eq "WM7_OSPlatform") { $branch = "WM7_OSPlatform\WM7_OSPlatform" }

# Find latest drop which contains the binsync zips we want
$dirs = gci \\build\release\7_Branch\$branch | ? { $_.PSIsContainer } | sort -prop LastWriteTime -descending
$dirs | % {
    $root = $_.FullName
    $binsyncroot = "$root\binsync"
    $deptreepath = "$binsyncroot\public\$deptree"
    if (test-path $deptreepath)
    {
        Extract-Zips-Under-Path $binsyncroot "$deptreepath\$subdir"
        
        $deptreepath = "$binsyncroot\private\$deptree"
        if (test-path $deptreepath)
        {
            Extract-Zips-Under-Path $binsyncroot "$deptreepath\$subdir"
        }
        break
    }
}