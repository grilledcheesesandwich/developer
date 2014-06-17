param([string]$FileName)
$file = Resolve-Path $FileName
$target = $file.Path + "_old"
copy $file $target
$content = gc $target
$i = 0
for (; $i -lt $content.Length; $i++)
{
    if (($content[$i] -match "^#include") -or ($content[$i] -match "^using"))
    {
        $match = $Matches[0].Value
        $header = $content[$i].Substring($match.Length).Trim("<>`" ")
        Write-Host -NoNewline ("Trying without `t{0} ... " -f $header)
        $trimmed = $content[0..($i-1) + ($i+1)..$content.Length]
        $trimmed | Out-File $file -Encoding ascii
        build -c 2>&1 | Out-Null
        if ($LASTEXITCODE -eq 0)
        {
            "Succes, removing it"
            $content = $trimmed
            $i--
        }
        else
        {
            "Failure, line was necessary"
            $lastLog = dir build*.log | sort -Property LastWriteTime | select -Last 1
            copy $lastLog "build_$header.log" | Out-Null
        }
    }
}

$content | Out-File $file -Encoding ascii