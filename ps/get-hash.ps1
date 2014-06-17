param(
    [string] $file = $(throw 'a filename is required'),
    [string] $algorithm = 'SHA1'
)

$fileStream = [system.io.file]::openread((resolve-path $file))
$hasher = [System.Security.Cryptography.HashAlgorithm]::create($algorithm)
$hash = $hasher.ComputeHash($fileStream)

[system.bitconverter]::tostring($hash)
$hash