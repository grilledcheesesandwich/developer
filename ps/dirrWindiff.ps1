if ($null -eq $args[0]) { return }

$files = gci -recurse -include $args
for ($i = 0; $i -lt $files.Count; $i++)
{
    for ($j = $i + 1; $j -lt $files.Count; $j++)
    {
        windiff $files[$i].FullName $files[$j].FullName
    }
}
#Get-Process windiff* | Stop-Process