Get-ChildItem -Recurse -Filter "*.cs" | % {
    $text = [System.IO.File]::ReadAllText($_.FullName)
    $output = $text.Replace("WellKnownAccounts.Supervisor", "WellKnownAccounts.ElevatedRights")
#	$output = $output.Replace("supervisor", "elevatedRights")
	$output = $output.Replace("WellKnownAccounts.Normal", "WellKnownAccounts.StandardRights")
#	$output = $output.Replace("normal", "standardRights")
   
	if ($output -ne $text)
	{
		sd.exe edit $_.FullName
		[System.IO.File]::WriteAllText($_.FullName, $output)
	}
}