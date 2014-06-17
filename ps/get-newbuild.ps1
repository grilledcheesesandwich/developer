param(
    [string] $flav = "Smartfon",
    [string] $sku = "ram_gsm_fakeril",
    [string] $branch = "Seven\Mainline",
    [string] $lang = "USA",
    [int]    $build,
    [switch] $all,
    [switch] $whatif = $false,
    [string] $debug = $env:wincedebug
)

$arch = $env:_TGTCPU
$proj = $env:_TGTPROJ
$plat = $env:_TGTPLAT
#$dbg  = $env:wincedebug
$frd = $env:_flatreleasedir

#$usefulExts = "*.exe *.dll *.mui *.map *.pdb *.cpl"
#$usefulExts = ("*.exe", "*.dll", "*.mui", "*.map", "*.pdb", "*.cpl", "*-diskimage.nb0")
$usefulExts = ("*.exe", "*.dll", "*.mui", "*.map", "*.pdb", "*.cpl")



# \\build\release\crossbow\15217\Selfhost\ARMV4I\smartfon\DEVICEEMULATOR\retail
# \\wcebinshop\images\crossbow\15321\DEVICEEMULATOR\ARMV4I
$buildRoot = "\\binshop\release\$branch"

# get all build folders
$dirs = get-childitem $buildRoot | where-object { $_.PsIsContainer -and $_.Name.Length -gt 3}

$buildNum = 0;


if ($build -eq 0)
{
    # calculate which one is the biggest
    $dirs | % {
      [UInt32] $build = [UInt32]::Parse($_.Name)
      if ($build -gt $buildNum)
      {
        "checking build $build"
        
        # check doneness
        $donefile = gci "$buildRoot\$build" -filter "*$plat*.done"
        
        if ($donefile -ne $null ) {
         $buildNum = $build 
         "accepted build $buildnum"
         }
        else
        {
            " no donefile"
        }
      }
    }
    
    if ($buildNum -eq 0)
    {
        "No finished build detected."
        [int]$buildNum = read-host "What build number do you want?"
    }
    
    $build = $buildNum

    write-host "Newest build is $build"
}

# build-release build pattern
#$global:buildDir = "$buildRoot\$build\Selfhost\$arch\$proj\$plat\retail"
# wcebinshop build pattern
$global:binsDir = "$buildRoot\$build\$plat\$arch"

"$binsDir"

# no flavor specified - w
#if ($args.Count -lt 1)
if ($flav -eq "")
{
    "No platform flavor specified."
    "Choices are: "
    $flavs = gci $binsDir
    foreach ($flavor in $flavs)
    {
        $flavor.Name
    }
    
    #return
    $flav = read-host "What flavor do you want?"
}
else
{
    #$flav = $args[0]
}

$subBinsDir = $binsDir + "\$flav\$debug\$lang"

#if ($args.Count -lt 2)
if ($sku -eq "")
{
    
    "No sku specified."
    "Choices from $subBinsDir: "
    $skus = gci $subBinsDir -filter *.bin
    foreach ($_sku in $skus)
    {
        $_sku.Name.Replace(".bin", "")
    }
    
    #return
    $sku = read-host "Choose a SKU"
}
else
{
    #$sku = $args[1]
}

# recover the diskimage
if ($plat -eq "DEVICEEMULATOR" -or $plat -eq "CEPC")
{
    "Deploying diskimage"
    $targetImageName = $frd + "\nk.bin"
    $sourceImageName = "$subBinsDir\$sku" + ".bin"
    
    "Source is " + $sourceImageName
    "Target is " + $targetImageName
    
    if (-not $whatif)
    {
        copy-item $sourceImageName $targetImageName
    }
    
}


if ($flav -imatch "PPC")
{
    $flav = "WPC";
}
$sku = "Common"

$binsDir += "\$flav\$debug\$sku"
#$binsDir = $subBinsDir + "\SavedFiles\$sku"


# robocopy all useful files from there to the FRD
write-output "Executing robocopy $binsDir $frd $usefulExts"
$roboArgs = $binsDir,$frd
$roboArgs += $usefulExts

# $roboArgs
if ($all -and -not $whatif)
{
    robocopy $roboArgs
    del $frd\rillog.dll
}


