cls
gci -recurse | % {
    if ($_.PsIsContainer)
    {
        if ([string](gci $_.FullName) -match "sources")
        {
            $path = $_.FullName
            $sources = [string](gc $path\sources*)
            if ($sources -match "(?<=TARGETNAME=)\w+")
            {
                $targetName = $Matches[0]
            }
            $files = gci $_.FullName -filter *.c* | where { $_.Name -match "c(pp|s)?$" } | where { $_.Name -notmatch "assemblyinfo.cs" }
            foreach ($file in $files)
            {
                if ($sources -notmatch $file.Name)
                {
                    if($targetName -eq $file.BaseName)
                    {
                        continue;
                    }
                    
                    $file.FullName
                }
            }
        }
    }
}
