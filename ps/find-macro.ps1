$macro = $args[0] + '='
build -c -nmake /g
$paths = gc build.log | select-string "(?<=^BUILD.*Included: ).*" | % { $_.Matches[0].Value }
$paths | % { select-string $macro $_ } | select -unique