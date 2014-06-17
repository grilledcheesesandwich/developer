gci -recurse | % {
    if ($_.PSIsContainer -and ($_.FullName -notmatch "\\obj"))
    {
        $path = $_.FullName + '\' + "contents.oak"
        if (test-path $path)
        {
            pushd $_.FullName;
            if ((gc contents.oak) -match "ossectst")
            {
                $_.FullName;
            }
            popd
        }
    }
}