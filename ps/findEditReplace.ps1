gci -recurse -filter *.h | % {
    if (!$_.PSIsContainer) {
        $text = [string](gc $_.FullName);
        if ($text -match "INFCONTEXTCE")
        {
            sd edit $_.FullName;
            set-content $_.FullName ((gc $_) -replace 'INFCONTEXTCE','INFCONTEXT')
        }
    }
}