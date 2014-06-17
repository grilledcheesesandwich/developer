gci -recurse | % {
    if ($_.PSIsContainer -and ($_.FullName -notmatch "\\obj"))
    {
        pushd $_.FullName;
        if ((test-path contents.oak) -and ((gc contents.oak) -match "ossectst"))
        {
            $_.FullName;
            oakchk;
        }
        popd
    }
}