param([string]$password)
$certs = new-object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
$files = dir *.pfx
$certArray = @()
$files | foreach-object {
[Byte[]] $bytes = get-content -Encoding Byte $_
$certs.Import($bytes, $password, "Exportable")
}
$bytes = $certs.export("pfx", $password)
set-content -path "packedCerts.pfx" -Encoding Byte -Value $bytes