param([string] $targetName = $(Read-Host -prompt "TargetName"))

$newDir = "$env:_DEVDIR\code\$targetName"

if (Test-Path($newDir)) { return }

md $newDir
pushd $newDir

copy $env:_DEVDIR\code\template_exe\*
dir | % { attrib -r $_.FullName }
$targetFile = "$targetName.cpp"
ren template.cpp $targetFile
(gc sources) | % { $_ -replace "template", $targetName } | sc sources
(gc $targetFile) | % { $_ -replace "template", $targetName } | sc $targetFile
sd add *
ii $targetFile
