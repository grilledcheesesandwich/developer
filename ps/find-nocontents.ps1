cls
gci -recurse | % {
    if ($_.PsIsContainer)
    {
        if ($_.FullName -notmatch "obj")
        {
            if ([string](gci $_.FullName) -notmatch "contents.oak")
            {
                $path = $_.FullName
                $path
            }
        }
    }
}