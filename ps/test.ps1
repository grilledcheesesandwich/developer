$lastProgress = gc $env:_WINCEROOT\BuildProgress.lkb.log
while ($true)
{
    $progress = gc $env:_WINCEROOT\BuildProgress.log
    $totalPercent = $progress.Length / $lastProgress.Length * 100
    if ($totalPercent -ge 100) { break }
    $currentItem = $progress | select -Last 1
   
    write-progress -ID 1 -Activity "Building Enlistment (Phase I)" -Status $currentItem -PercentComplete $totalPercent
    Start-Sleep -Milliseconds 10000
}
while ($true)
{
    $progress = gc $env:_WINCEROOT\timelog.txt
    $currentItem = $progress | select -Last 1
    write-progress -ID 1 -Activity "Building Enlistment (Phase II)" -Status $currentItem
    Start-Sleep -Milliseconds 10000
}