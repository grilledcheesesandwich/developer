param([string]$drop = $(Read-Host -prompt "Drop location"))

gc build.err | % {
    if ($_ -match "Metadata file '([^']+)' could not be found")
    {
        $fi = new-object System.IO.FileInfo $Matches[1]
        copy $drop\$($fi.Name) $($fi.DirectoryName) -Verbose
    }
}