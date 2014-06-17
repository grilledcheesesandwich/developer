param([string]$BuildString)
if ([String]::IsNullOrEmpty($BuildString))
{
	$builds = dir \\build\Release\Apollo\$env:_RELEASELABEL | ? { $_.PSIsContainer } | sort -prop LastWriteTime
	$BuildString = $builds | select -last 1
}

Write-Host -ForegroundColor DarkGreen "Last Changelists for $BuildString"

$changeOutput = sdx changes -m 1 ..."@$BuildString"
if ($LASTEXITCODE -ne 0)
{
   throw  "`"sdx changes -m 1 ...@$BuildString`" failed"
}

$trimmed = $changeOutput | ? { $_.StartsWith("----------------") -or $_.StartsWith("Change ") } | % { $_.Split()[1] }
$branch = ""
for ($i = 0; $i -lt $trimmed.Length; $i++)
{
	if ($trimmed[$i] -match "\D")
	{
		"{0}: <= {1}" -f $trimmed[$i], $trimmed[++$i]
	}
}