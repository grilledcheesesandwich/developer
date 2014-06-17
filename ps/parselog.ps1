
function parseLog {
    param([string] $logname)

    # pattern to pull out the byte return #
    $pat = "Succeeded returning (\d+) bytes"
    
    # slurp the log
    $lines = get-content $logname
    
    # parse each line
    $global:values = @()
    foreach ($line in $lines) {
    
     # execute a regex match against each line
     $match = [Regex]::Match($line, $pat)
     
     # grab the number we matched and stuff it in our value array
     if ($match.Success) {
       $values += $match.Groups[1].Value
     }
    }
    
    # calculate some stats
    $measureObj = $($values | measure-object -Average -Sum -Max -Min)
    
    printPageStatistics
    
    # reformat the statistics object into something prettier

    #$global:statsObj = new-object Object     
    $global:statsObj = new-object System.Management.Automation.PSObject
    $statsObj | add-member NoteProperty "Total Bytes Read" $measureObj.Sum | out-null
    $statsObj | add-member NoteProperty "Total Reads" $measureObj.Count | out-null
    $statsObj | add-member NoteProperty "Average Read Size" $([Int32]$measureObj.Average) | out-null
    $statsObj | add-member NoteProperty "Largest Read" $([Int32]$measureObj.Max) | out-null

    # print out these statistics    
    $statsObj | format-table
    
    # Now create the grouped list of reads
    # group together by size of file operation
    $global:grouped = $($values | group-object | sort-object -property Count)
    
    # strip off the properties we're not interested in
    $grouped = $($grouped | select-object Name,Count)
    
    # group again by # of times for read operation
    # this collapses together all the reads that only happened once, etc. 
    $grouped = $($grouped | group-object Count)
    
    # now dump out the results to the output
    # I wasn't able to get format-table to change the column headers, so 
    # we just write out the column headers by hand
    "Times Read                                        Bytes Read"
    "----------                                        ----------"
    $grouped | format-table Name,Group  -HideTableHeaders
    
    

}

function printPageStatistics {
    
    # calculate the aggregate number of pages that were read
    $totalPages = [Int32] ($measureObj.Sum / 4k)

    # print out our calculations in-place
    
    # Because of the key block and the encryption guid, all files have at least two 
    # extra pages
    "Minimum pages read (one read operation): $($totalPages+2)"
    
    # Each read requires at least two reads because the keyblock must be read
    "Minimum pages read (page-size reads): $($totalPages*2)"
    "Your pages read (optimistic calculation): $($measureObj.Count*2)"
}


############# main ###########################

# parse the input

if ($args.Count -ne 1)
{
    # print out usage message
    ""
    "Usage: parselog <fsdspy log name>"
    ""
}
else
{
    parseLog($args[0])
}