#   ^(static)?\s*\w+(\s+|\s*\*\s*)\w+\([^\)]*\)\s*{
type .\functions.txt | % { $matches = @(gci -recurse -filter *.c* | select-string $_); $_ + "`t" + $matches.Count } > matches.csv