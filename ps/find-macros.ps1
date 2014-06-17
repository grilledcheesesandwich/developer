cls
$macros = New-Object System.Collections.Generic.List[System.String]
gci -recurse -filter sources* | select-string "(?<=\$\()_[^\)]+" | % {
    $macro = $_.Matches[0].Value
    if (!$macros.Contains($macro))
    {
        $macros.Add($macro)
    }
}
$macros.Sort()
$macros