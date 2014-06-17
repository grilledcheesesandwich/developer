param ( [string]$timeFile = "$env:_WINCEROOT\timelog.txt" )

$lines = get-content $timeFile
    
#$line = "11:10 (0) sysgen -p shellw -b preproc"
$lineReg = [regex] "(.*) \((\d+)\) (.*)"


$lines | % {
    if ($_ -match $lineReg)
    {
        $timeStamp = [datetime]$matches[1]
        $elapsed = [int]$matches[2]
        $operation = $matches[3].trim()

        # in this line we learn about an operation, the time it started, and the
        # elapsed time of the previous operation
        $operationObj = new-object System.Management.Automation.PSObject
        $operationObj | add-member NoteProperty "Operation" $operation | out-null
        #$operationObj | add-member NoteProperty "TimeStamp" $timeStamp | out-null

        if ($lastOperation)
        {
            $lastOperation | add-member NoteProperty "ElapsedTime" $elapsed | out-null
            
            $lastOperation
        }
        
        $lastOperation = $operationObj
        
        
        #$prevElapsed = $elapsed
        
    }
    else
    {
        write-host "no match : $_"
    }
}

parse-timelog.ps1 $env:_WINCEROOT\timelog.txt | ? {$_.ElapsedTime -gt 3} | sort -prop ElapsedTime | ft Operation,ElapsedTime
