[xml]$perf = gc $env:_FLATRELEASEDIR\LoaderVerifierPerfResults.xml
"File,IsModule,Size,Authn,Routing,Authz,FlushedAuthn,FlushedRouting,FlushedAuthz"
$perf.PerfScenarioLog.ScenarioInstance | % {
    $tokens = $_.Scenario.Name.Split(':')
    $file = $tokens[1]
    $size = $tokens[2]
    $isModule = $tokens[3]
    $isFlushed = $tokens.Length -eq 5
    $authn = 0
    $route = 0
    $authz = 0
    $_.SessionNamespace.SessionNamespace.Duration | % {
        if ($_.Name -eq "LvMod_Authenticate") { $authn = $_.Average / 1000 }
        if ($_.Name -eq "LvMod_Routing") { $route = $_.Average / 1000 }
        if ($_.Name -eq "LvMod_Authorize") { $authz = $_.Average / 1000 }
    }
    
    if (!$isFlushed)
    {
        $firstChunk = "{0},{1},{2},{3},{4},{5}" -f $file, $isModule, $size, $authn, $route, $authz
    }
    else
    {
        "{0},{1},{2},{3}" -f $firstChunk, $authn, $route, $authz
    }
} 