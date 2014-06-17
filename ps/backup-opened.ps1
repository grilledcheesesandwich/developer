param([string]$SourceRoot = $(Read-Host -prompt "Enlistment root to back up"),
      [string]$TargetDirectory = $(Read-Host -prompt "Back up files to"))
$TargetDirectory = $TargetDirectory.TrimEnd('\') 

$env:SDXROOT = $SourceRoot
$env:Path += ";$SourceRoot\public\coresystem\tools\perl\bin\;$SourceRoot\nttools\x86"
$env:_WINPHONEDRIVE = $SourceRoot.Substring(0, 2)
$opened = &$SourceRoot\tools\bat\sdx.cmd opened -l -nosummary
if (!$?)
{
    Write-Host -ForegroundColor Red "Error running sdx opened"
    return
}

$wpRoot = new-object System.IO.DirectoryInfo $SourceRoot
$targetRoot = [System.IO.Path]::Combine($TargetDirectory, $wpRoot.Name)
#if (Test-Path $targetRoot)
#{
    #del -r $targetRoot
#}
$targetRoot += " [{0}]" -f ([Math]::Round([DateTime]::Now.Ticks / [TimeSpan]::TicksPerSecond))

$opened | % {
    $match = [System.Text.RegularExpressions.RegEx]::Match($_, "(.*)#")
    if ($match.Success -and !$_.Contains("- delete"))
    {
        $match.Groups[1].Value
    }
    } | % {
        $target = new-object System.IO.FileInfo $_.Replace($SourceRoot, $targetRoot)
        if (!$target.Directory.Exists)
        {
            $null = md $target.DirectoryName
        }
        $result = copy -v $_ $target.FullName
        if (!$?)
        {
            Write-Host -ForegroundColor Red $result
        }
        else
        {
            $result
        }
    }
Write-Host "Press any key to continue ..."
$null = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
