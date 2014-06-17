$makecert = "$env:_PRIVATEROOT\test\mobile\areas\itt\security\bin\makecert.exe"
$pvkpfx = "$env:_PRIVATEROOT\test\mobile\areas\itt\security\bin\pvkpfx.exe"

$name = read-host "Name"
$answer = read-host "Self-signed? (y/n)"
$isSelfSigned = $answer.StartsWith("y") -or [string]::IsNullOrEmpty($answer)
if ($isSelfSigned)
{
    $issuer = "-r "
}
else
{
    $issuer = read-host "Issuer file (no extension)"
    $issuer = "-ic {0}.cer -iv {0}.pvk" -f $issuer
}
$filename = read-host "Out filename (no extension)"
$algorithm = read-host "Algorithm (SHA1 [default]/MD5)"
if ([string]::IsNullOrEmpty($algorithm)) { $algorithm = "sha1" }
$ekus = read-host "Comma-delimited list of EKUs [1=Server Auth, 2=Client Auth, 3=Code Signing]"
$ekus = $ekus.Split(',')
$eku = ""
$ekus | % {
    switch ($_)
    {
        "1" { $eku += "1.3.6.1.5.5.7.3.1," }
        "2" { $eku += "1.3.6.1.5.5.7.3.2," }
        "3" { $eku += "1.3.6.1.5.5.7.3.3," }
    }
}
if (!([string]::IsNullOrEmpty($eku))) { $eku = "-eku " + $eku }
$len = read-host "Key Length (default=2048)"
if ([string]::IsNullOrEmpty($len)) { $len = "2048" }

$arg = "{0} -sky exchange -n CN={1} -a {2} {3} -len {4} -sv {5}.pvk {5}.cer" -f $issuer, $name, $algorithm, $eku, $len, $filename
"makecert.exe $arg"
&$makecert $arg.Split(' ')
$password = read-host "Password for the PFX"
$arg = "-pwd {0} -cert {1}.cer -pvk {1}.pvk -pfx {1}.pfx -out pfx" -f $password, $filename
&$pvkpfx $arg.Split(' ')