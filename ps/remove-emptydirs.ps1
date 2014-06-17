function IsEmpty
{
    param($subdir)
    
    return (($subdir.getfiles().count -eq 0) -and ($subdir.getdirectories().count -eq 0))
}

$dirs = New-Object System.Collections.Generic.Queue[IO.DirectoryInfo]
write-progress -Activity "Removing Empty Directories" -status "First pass: Found $($dirs.Count) empty directories"
gci -recurse | ? {$_.psiscontainer} | ? {IsEmpty $_} | % { $dirs.Enqueue($_); write-progress -Activity "Removing Empty Directories" -status "First pass: Found $($dirs.Count) empty directories" }

$i = 0
while ($dirs.Count -gt 0)
{
    $toDelete = $dirs.Dequeue()
    $status = "{0,5} Deleting {1}" -f ++$i, $toDelete.FullName
    write-progress -Activity "Second pass: Removing Empty Directories" -status $status
    $toDelete.Delete()
    if ((IsEmpty $toDelete.Parent) -and (!$dirs.Contains($toDelete.Parent)))
    {
        $dirs.Enqueue($toDelete.Parent)
    }
}
"Deleted $i empty directories"