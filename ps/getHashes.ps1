$certs = new-object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
$files = dir *.pfx
$certArray = @()
$files | foreach-object {
[Byte[]] $bytes = get-content -Encoding Byte $_
$certs.Import($bytes, "password", "Exportable")
}
$certs | foreach-object {
Add-content -path "hashes.txt" -Encoding Ascii -Value $_.Thumbprint
}