param([switch]$Includes)
copy sources "sources_old" -Force
$sources = gc sources_old
$i = 0
for (; $i -lt $sources.Length; $i++)
{
    if ($Includes)
    {
        if ($sources[$i] -match "INCLUDES")
        {
            break
        }
    }
    if ($sources[$i] -match "TARGETLIBS")
    {
        break
    }
    if ($sources[$i] -match "MANAGED_REFERENCES")
    {
        $i--
        break
    }
}

while ($sources[++$i] -match "\w+")
{
    $lib = ($sources[$i].Split(@('\'), "RemoveEmptyEntries") | Select -Last 1).Trim()
    Write-Host -NoNewline ("Trying without `t{0} ... " -f $lib)
    $trimmed = $sources[0..($i-1) + ($i+1)..$sources.Length]
    $trimmed | Out-File sources -Encoding ascii
    build -c 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0)
    {
        "Succes, removing it"
        $sources = $trimmed
        $i--
    }
    else
    {
        "Failure, line was necessary"
        $lib = ($sources[$i].Split(@('\'), "RemoveEmptyEntries") | Select -Last 1).Trim()
        $lastLog = dir build*.log | sort -Property LastWriteTime | select -Last 1
        copy $lastLog "build_$lib.log" | Out-Null
    }
}

$sources | Out-File sources -Encoding ascii