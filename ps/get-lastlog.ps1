gci $Env:_FLATRELEASEDIR -filter results*.log | sort-object -prop LastWritetime | select-object -last 1 | % { tatlog $_.FullName }
