function EvaluateMacro($macroNode)
{
    $macro = $macroNode.Source
	if ($macro -match "^\$\((\w+)\)$")
	{
		$macro = $Matches[1]
        $macroNode.Source = $xml.SelectSingleNode("//ns:Macros/ns:Macro[@Id='$macro']", $mgr).Value
        return $macroNode
	}
	else
	{
		return $macroNode
	}
}

function GetFiles($path, $token)
{
	try
	{
		$xml = [xml](gc $path)
		$mgr = new-object System.Xml.XmlNamespaceManager($xml.Psbase.NameTable)
		$null = $mgr.AddNamespace("ns",$xml.Package.psbase.NamespaceURI)
		$files = $xml.SelectNodes("//ns:OSComponent//ns:File | //ns:Package/ns:Files/ns:File", $mgr)
        $evaluatedFiles = $files | % { EvaluateMacro($_) }
		$exes = @($evaluatedFiles | ? { $_.Source -match "\.exe$" })
        if ($exes.Count -gt 0)
        {
            #$exes | ? { $_.DestinationDir -match $token } | ? { $_ -ne $null }
    		#$exes | ? { $_.DevicePath -match $token } | ? { $_ -ne $null }
            #$exes | select -Property DevicePath | ? { $_ -match "tuxnet" } | % {
                #$_
            #}
            $exes | ? { $_.DestinationDir -ne $null -and $_.DestinationDir -notmatch "(TEST_DEPLOY_PROJ)|(TEST_DEPLOY_BIN)" } | ? { $_ -ne $null }
    		$exes | ? { $_.DevicePath -ne $null -and $_.DevicePath -notmatch "(TEST_DEPLOY_BIN|TEST_DEPLOY_ROOT)" } | ? { $_ -ne $null }
    		$exes | ? { $_.Attributes.Count -eq 1 } | ? { $_ -ne $null }
        }
	}
	catch
	{
		Write-Error $path
		$path
	}
}

function GetOwner($path)
{
    $dir = New-Object IO.DirectoryInfo (Split-Path $path)
    $coak = "$dir\contents.oak"
    while (!(test-path $coak))
    {
        $previousDir = $dir
        $dir = $dir.Parent
        if ($previousDir -eq $dir) { return }
        $coak = "$($dir.FullName)\contents.oak"
    }
    $owner = gc $coak | select-string "(?<=TEAMOWNER=)[\.\w]+"
    if ($owner.Matches -eq $null)
    {
        return "unknown"
    }
    $owner.Matches[0].Value
}

$packages = (cidir *.proj.xml) + (cidir *.pkg.xml)
#$packages = @("F:\OSPlt8BaseOSSEC1\src\tools\TestInfra\Product\harnesses\packages\tuxnet\tuxnet.proj.xml")

$packages | % {
    $package = split-path $_ -leaf
    $owner = GetOwner $_
    
    #$system32Files = GetFiles $_ "system32"
    #$testFiles = GetFiles $_ "\\test"
    #$files = @($system32Files) + @($testFiles)
    $files= GetFiles $_ "foo"
    
    $files | % {
        if (![string]::IsNullOrEmpty($_))
        {
            "{0}`t{1}`t{2}`t{3}{4}" -f $owner, $package, $_.Source, $_.DevicePath, $_.DestinationDir
        }
    } | Select -Unique
}