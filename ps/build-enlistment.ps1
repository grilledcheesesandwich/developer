function Get-MatchLineNumber($current)
{
    for ($i = 0; $i -lt $lines.Length; $i++)
    {
        if ($lines[$i].Contains($current))
        {
            return $i
        }
    }
}

function Get-LineTime
{
    param([string]$line)
    
    $line -match $rgx | out-null
    $parts = $matches[1].Split(':')
    
    if ($parts.Length -eq 2)
    {
        $time = new-timespan -minutes $parts[0] -Seconds $parts[1]
    }
    else
    {
        $time = new-timespan -hours $parts[0] -minutes $parts[1] -Seconds $parts[2]
    }
    
    # return
    $time
}

# start-job -ScriptBlock { d:\WM7CoreOS\public\common\oak\misc\blddemo.bat }
#ii d:\WM7CoreOS\public\common\oak\misc\blddemo.bat

$root = $env:_WINCEROOT
$timelog = "$root\timelog.txt"
$timelogClean = "$root\timelog.clean.txt"

if (!(test-path($timelogClean)))
{
    copy $timelog $timelogClean
}

$rgx = "\[(.*)\]\s+(.*)"

$lines = gc $timelogClean
$lastLine = $lines | select -last 1
$totalTime = Get-LineTime($lastLine)
$swTotal = [Diagnostics.Stopwatch]::StartNew()
while ($true)
{
    $currentLines = gc $timelog
    $currentLine = $currentLines | select -last 1
    $currentLine -match $rgx | out-null
    $currentStep = $matches[2]

    # if we've reached the end of the build, display last line and exit
    if ($lastLine -match $currentStep) { $currentLine; return }
    
    # We need to get the step AFTER the "current" line as that's the one that is actually happening
    $matchedLineNumber = Get-MatchLineNumber($currentStep)
    $matchedLineNumber++
    $matchedLine = $lines[$matchedLineNumber]
    $stepTime = Get-LineTime($matchedLine)
    
    # Get expected time remaining
    $timeRemaining = new-timespan
    for ($i = $matchedLineNumber + 1; $i -lt $lines.Length - 1; $i++)
    {
        $lineTime = Get-LineTime($lines[$i])
        $timeRemaining += $lineTime
    }
    $totalPercent = $currentLines.Length / $lines.Length * 100
    
    # The loop for a single build step
    $swStage = [Diagnostics.Stopwatch]::StartNew()
    $newCurrentLine = $currentLine
    while ($currentLine -eq $newCurrentLine)
    {        
        $stepPercent = $swStage.Elapsed.TotalSeconds / $stepTime.TotalSeconds * 100;
        
        if ($stepPercent -gt 100) { $stepPercent = 100 }
        
        write-progress -ID 1 -Activity "Building Enlistment" -Status "Total Progress:" -PercentComplete $totalPercent -SecondsRemaining ($timeRemaining.TotalSeconds - $swStage.Elapsed.TotalSeconds)
        write-progress -ID 2 -Activity $currentStep -Status "Stage Progress:" -PercentComplete $stepPercent -SecondsRemaining ($stepTime.TotalSeconds - $swStage.Elapsed.TotalSeconds)
        
        Start-Sleep -Milliseconds 1000
        
        $currentLines = gc $timelog
        $newCurrentLine = $currentLines | select -last 1
    }
}