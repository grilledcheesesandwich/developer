param([switch] $recurse,
      [switch] $openOnly,
      [switch] $copyBinaryPath,
      [switch] $copyImagePath,
      [string] $filespec,
	  [string] $branch,
      [string] $label,
      [string] $language = "USA")

if ($recurse) { $r = "/s" }

# If user specified a label, we get the path from there
if (![string]::IsNullOrEmpty($label))
{
    # Get the branch from the label
    $branch = $label.Split('.')[0]

    $binaryPath = "\\build\release\Blue\$branch\{0}\{1}.{2}{3}\Binaries" -f $label, $env:_TGTOS, $env:_BUILDARCH, $env:BUILDTYPE
    $imagePath = "\\build\release\Blue\$branch\{0}\{1}.{2}{3}\Binaries\Images\vm_allres\Test\{4}" -f $label, $env:_TGTOS, $env:_BUILDARCH, $env:BUILDTYPE, $language
}
else
{
    # If branch isn't specified, get it from the build window
    if ([string]::IsNullOrEmpty($branch))
    {
        $branch = $env:_RELEASELABEL
    }

    # Get all drop points, sort by date
    $dirs = gci \\build\release\Blue\$branch | ? { $_.PSIsContainer } | sort -prop LastWriteTime -descending

    # Find the most recently populated drop
    foreach ($dir in $dirs)
    {
        $binaryPath = "{0}\{1}.{2}{3}\Binaries" -f $dir.FullName, $env:_TGTOS, $env:_BUILDARCH, $env:BUILDTYPE
        $imagePath = "{0}\{1}.{2}{3}\Binaries\Images\vm_allres\Test\{4}" -f $dir.FullName, $env:_TGTOS, $env:_BUILDARCH, $env:BUILDTYPE, $language
        
        if ($copyBinaryPath -and (test-path $binaryPath))
        {
            break
        }
        elseif ((test-path "$imagePath\flash.vhd") -or (test-path "$imagePath\bootablemobile.vhd"))
        {    
            break;
        }
    }
}

if ($openOnly)
{
    ii $imagePath
}
elseif ($copyBinaryPath)
{
    $binaryPath
    $binaryPath | setclip
}
elseif ($copyImagePath)
{
    "$imagePath\flash.vhd"
    "$imagePath\flash.vhd" | setclip
}
else
{
    # Copy images or $filespec
    if ([string]::IsNullOrEmpty($filespec))
    {
        robocopy $imagePath $env:_FLATRELEASEDIR flash_debug.vhd flash.vhd $r
    }
    else   
    {
        robocopy $imagePath $env:_FLATRELEASEDIR $filespec $r
    }
}