param([string]$path,
      [string]$password = "")

$certs = new-object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
$certs.Import((resolve-path $path), $password, "Exportable")
if (!(test-path .\certs)) { md certs }
$certs | % {
	$type = "inter"
	if ($_.HasPrivateKey) { $type = "leaf" }
	elseif ($_.Subject -eq $_.Issuer) { $type = "root" }
    $_.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Cert) | set-content ("certs\{0}_$type.cer" -f $_.Thumbprint) -Encoding Byte
}