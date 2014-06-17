param($path=$env:_FLATRELEASEDIR)

$signature = @' 
[DllImport("Crypt32.dll", SetLastError = true)]
public static extern IntPtr CryptGetMessageCertificates(
    uint dwMsgAndCertEncodingType,
    IntPtr hCryptProv,
    uint dwFlags,
    Byte[] pbSignedBlob,
    Int32 cbSignedBlob);
'@ 
$type = Add-Type -MemberDefinition $signature -Name Win32Utils -PassThru 


$xapCode = [IO.File]::ReadAllText("$env:_DEVDIR\ps\XAP.cs")
Add-Type -TypeDefinition $xapCode -Language CSharpVersion3

$files = dir -filter *.xap -path $path -Recurse
foreach ($file in $files)
{
    try
    {
        $xap = new-object Microsoft.MobileDevices.AreaLibrary.Security.LoaderVerifier.XAP $file.FullName
    }
    catch { "{0}`tEXCEPTION PARSING" -f $file.FullName; continue }
    if ($xap.IsSigned)
    {
        foreach ($desc in $xap.Descriptors)
        {
            if ($desc.Type -eq [Microsoft.MobileDevices.AreaLibrary.Security.LoaderVerifier.XAP]::XAP_SIGNATURE_TYPE_AUTHENTICODE)
            {
                $hStore = $type::CryptGetMessageCertificates(0, [IntPtr]::Zero, 0, $desc.Data, $desc.Data.Length)
                $store = new-object Security.Cryptography.X509Certificates.X509Store $hStore
                if ($store.Certificates.Count -eq 1) { $cert = $store.Certificates[0] }
                else
                {
                    $last = $store.Certificates | select -Last 1
                    $previous = $store.Certificates | select -last 1 -skip 1
                    if ($last.Issuer -eq $previous.Subject) { $cert = $last }
                    elseif ($last.Subject -eq $previous.Issuer) { $cert = $previous }
                    else { Write-Error ("{0}: No leaf cert found" -f $file.FullName) }
                }
                $ekus = $cert.Extensions | ? { $_.Oid.FriendlyName -eq "Enhanced Key Usage" } | select -Property EnhancedKeyUsages
                $ekus = $ekus.EnhancedKeyUsages | ? { $_.FriendlyName -ne "Code Signing" -and $_.FriendlyName -ne "AppStore" }
                $ekuString = ""
                $ekus | % { $ekuString += "$($_.FriendlyName)," }
                "{0}`t{1}`t{2}" -f $file.FullName, $cert.Subject, $ekuString
            }
        }
    }
    else
    {
        "{0}`tUnsigned" -f $file.FullName
    }
}