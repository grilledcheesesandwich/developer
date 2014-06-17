param($changelist)
$description = sd describe -D $changelist
$matches = [Text.RegularExpressions.Regex]::Match($description, "(\d{4}/\d{2}/\d{2}) (\d{2}:\d{2}:\d{2})")
$dateTime = "{0}:{1}" -f $matches.Groups[1].Value, $matches.Groups[2].Value
$release = $env:_RELEASELABEL
RIBugTracker -u -f `"FixAvailable Branch`"=`"$release`" $dateTime-$dateTime