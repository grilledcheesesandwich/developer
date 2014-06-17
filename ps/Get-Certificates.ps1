param([string] $filename, $password)

$filename = resolve-path $filename
$certs = new-object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
$certs.Import($filename, $password, "Exportable")
       
# return
$certs