$root = "http://msdiningapp/cafemenus/"
$rootPage = Invoke-WebRequest -Uri $root
foreach ($link in $rootPage.Links)
{
    $cafeName = $link.href.trim('/')
    $jsUrl = ("{0}menus/{1}.js" -f $root, $cafeName)
    try
    {
        $cafeScriptPage = Invoke-WebRequest -Uri $jsUrl
        $cafeScript = $cafeScriptPage.Content
        $regex = "function (\w*)Menu[^}]*$args"
        
        $allMatches = $cafeScript | ss $regex -allmatches
        if ($allMatches.Count -gt 0)
        {
            $allDays = $allMatches | % { $_.Matches } | % { $_.Groups[1].Value }
            "{0}: {1}" -f $cafeName, [String]::Join(", ", $allDays)        
        }        
    }
    catch
    {
        #Write-Host -ForegroundColor Red ("{0}: {1}: BAD URL" -f $cafeName, $jsUrl)
    }
}