"Search Helpers loaded"

function global:cidir([string] $filename)
{
    $filename = $filename.Replace("*", "%")
    $where = "System.ItemName LIKE '$filename'"
    $results = query $where | sort
	$results | select -last 1 | setclip
	$results
}

function global:cip([string] $text, [string] $filename)
{
    if (![String]::IsNullOrEmpty($filename))
    {
        $filename = $filename.Replace("*", "%")
        $where = "CONTAINS(Contents, '$text') AND System.ItemName LIKE '$filename'"
    }
    else
    {
        $where = "CONTAINS(System.Search.Contents, '$text')"
    }
    $results = query $where | sort
	$results | select -last 1 | setclip
	$results
}

function global:query([string] $where)
{
    $currentRoot = $env:_WINPHONEROOT
    $sql = "SELECT TOP 100 System.ItemPathDisplay
            FROM SYSTEMINDEX
            WHERE SCOPE = '$currentRoot' AND System.ItemType <> 'Directory' AND $where"
    #$sql
    $Provider="Provider=Search.CollatorDSO;Extended Properties=’Application=Windows’;"
    $adapter = new-object system.data.oledb.oleDBDataadapter -argument $sql, $Provider
    $ds      = new-object system.data.dataset
    if ($adapter.Fill($ds))
    {
        $ds.Tables[0] | % {
           $_."SYSTEM.ITEMPATHDISPLAY"
        }
    }
}
