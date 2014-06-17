$researchHelpers = New-WebServiceProxy -uri http://msitresearch/kmCodeService.asmx -UseDefaultCredential

"reSearch Helpers loaded"

function global:cidir([string] $filename)
{
    $where = "filename:$filename"
    $results = query $where | sort
	$results | select -last 1 | setclip
	$results
}

function global:cip([string] $text, [string] $filename)
{
    if (![String]::IsNullOrEmpty($filename))
    {
        $where = "$text AND filename:$filename"
    }
    else
    {
        $where = "$text"
    }
    $results = query $where | sort
	$results | select -last 1 | setclip
	$results
}

function global:query([string] $where)
{
    $collection = "wpmain"
    $currentRoot = $env:_WINPHONEROOT
    $results = $researchHelpers.Query("$collection(wp)", $where, "\", 0, [int]::MaxValue)
    $results | % { translateDepotPath $_.Filename $collection }
}
