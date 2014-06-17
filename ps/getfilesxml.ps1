param([string]$folder)

$template = @"
<File
  Source=`"`$(_WINPHONEROOT)\src\devmgmt\dm\test\dmsupportfiles\csp\{0}`"
  DestinationDir=`"`$(TEST_DEPLOY_PROJ)\csp\{1}`" />
"@

$root = "$env:_WINPHONEROOT\src\devmgmt\dm\test\dmsupportfiles\csp\"
$fullPath = $root + $folder
$files = dir -recurse $fullPath
$files | % {
    $trimmed = $_.FullName.Substring($root.Length)
    $template -f $trimmed, $folder
}