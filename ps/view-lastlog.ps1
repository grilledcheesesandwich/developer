getd results.log $env:temp
deld results.log
#$lastLog = dir $env:temp\tuxnet_*.log | sort -Property LastWriteTime | select -Last 1 -Skip 1
#$lastLogCopy = "$env:temp\lastLog" + $lastLog.Extension
#copy $lastLog $lastLogCopy
if ((gps TextAnalysisTool.NET) -eq $null)
{
    tl "$env:temp\results.log"
}