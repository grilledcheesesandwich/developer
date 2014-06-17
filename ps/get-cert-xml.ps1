# CertificateStore template for adding a ROOT cert
$certAddString = @"
<wap-provisioningdoc> 
    <characteristic type="CertificateStore"> 
        <characteristic type="ROOT"> 
            <characteristic type="{0}"> 
                <parm name="EncodedCertificate" value=" 
                    {1}
                "/> 
            </characteristic> 
        </characteristic> 
    </characteristic> 
</wap-provisioningdoc> 
"@

# Load in a .CER file
$cert = get-pfxcertificate $args[0]

$signerName = $cert.Subject.Substring(3)

write-host("Creating add XML for $signerName")
write-host

# get the thumbprint
$certHash = $cert.GetCertHashString()

# Convert the encoded blob to base64 text
$encodedCertificate = [Convert]::ToBase64String($cert.GetRawCertData())

# print those into our WAP xml template
$outXml = $certAddString -f ($certHash, $encodedCertificate)

# finished - write the XML to the outbound pipeline
write-output $outXml