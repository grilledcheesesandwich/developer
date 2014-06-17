param([string]$platform = "Tsunagi",
      [string]$targetShare = "\\keendres1\share")

# Get all drop points, sort by date
$dirs = gci \\build\release\7_Branch\WM7_Main | ? { $_.PSIsContainer } | sort -prop LastWriteTime -descending

$dirs | % {
    # Copy image if it doesn't already exist
    $path = "{0}\{1}\{2}\{3}\{4}\{5}" -f $_.FullName, $platform, "armv7", "WM_Premium_Min", "retail", "USA"
    
    $targetName = "{0}_FlashClean.ffu" -f $_.Name
    if (test-path "$path\FlashClean.ffu")
    {
        if (test-path "$targetShare\$targetName")
        {
            "No new images were found"
        }
        else
        {
            "Copying $path\FlashClean.ffu to $targetShare\$targetName"
            copy "$path\FlashClean.ffu" "$targetShare\$targetName"
        }
        break
    }
}