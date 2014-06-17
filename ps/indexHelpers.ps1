[void] [Reflection.Assembly]::LoadFile("$env:_DEVDIR\ps\CatalogQueryManager.dll")
"Index Helpers loaded"

function global:cidir([string] $filename, [switch] $original)
{
    $filename = $filename.Replace("*", "%")
    $where = "FILENAME LIKE '$filename'"
    $results = query $where -original:$original | sort
	$results | select -last 1 | setclip
	$results
}

function global:ciedit([string] $filename, [switch] $original)
{
    cidir @psBoundParameters | % { ii $_ }
}

function global:cip([string] $text, [string] $filename, [switch] $original)
{
    if (![String]::IsNullOrEmpty($filename))
    {
        $filename = $filename.Replace("*", "%")
        $where = "CONTAINS(Contents, '$text') AND FILENAME LIKE '$filename'"
    }
    else
    {
        $where = "CONTAINS(Contents, '$text')"
    }
    $results = query $where -original:$original | sort
	$results | select -last 1 | setclip
	$results
}

function global:cipedit([string] $text, [string] $filename, [switch] $original)
{
	cip @psBoundParameters | % { ii $_ }
}

function global:query([string] $where, [switch] $original)
{
    $currentRoot = $env:_RELEASELABEL
    #$servers = @("undev", "wpcsearch01")
    #$catalogs = @($currentRoot, "WP8 Group 01")
    #$roots = @($currentRoot, "apollo\\wpmain")
    $servers = @("wpcsearch01")
    $catalogs = @("WP8 Group 01")
    $roots = @("apollo\\wpmain")

    for ($i = 0; $i -lt $servers.Length; $i++)
    {
        try
        {
            $query = [CatalogQueryManager.CatalogQuery]::RemoteQuery($servers[$i], $catalogs[$i], "SELECT * FROM FILEINFO WHERE $where")
        }
        catch [System.Management.Automation.MethodInvocationException]
        {
            write-host $_.Exception
            continue;
        }
        
        while ($query.Read())
        {
            $path = $query.GetValue(0).ToString()
            if (!$original)
            {
                $path = $path -replace ("\\\\{0}\\{1}" -f $servers[$i], $roots[$i]), $env:_WINPHONEROOT
            }
            $path
        }
        $query.Close()
        break;
    }
}