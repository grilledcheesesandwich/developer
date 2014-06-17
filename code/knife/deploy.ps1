# big install
deploy-binary certtool.exe
deploy-device -packages Microsoft.Phone.Test.BaseOS.CertInstallerTests.spkg -packagerootpath $Env:BINARY_ROOT
# deploy new internalfx
put-binary Microsoft.Phone.Internal.dll

# incremental update
put-binary certtool.exe

$dataPath = "c:\data\test\baseos\certinstaller"

# CER file tests
cmdd certtool verifycertfile $dataPath\cer\base64\usercer.b64.cer

# root cert
verify-file $dataPath\cer\base64\chsroot.b64.cer

# manual installation
cmdd certtool addcert $dataPath\cer\base64\chsroot.b64.cer ROOT LocalMachine
# confirm present
cmdd certtool dc ROOT 
# manual cleanup
cmdd certtool removecert 89D775AF5919755533A9DA3D88B810A92F33113C root 

# p7b tests
verify-file ($dataPath + '\p7b\BigChain$c50.p7b')
verify-file ($dataPath + '\p7b\chain$c5.p7b')

# pfx tests
verify-file ($dataPath + '\pfx\compat\w98$wie4$av$c3$erb$p123456.pfx')

# 8 KB keys
verify-file ($dataPath + '\pfx\failing\nt5$wmmc$p$ere$c3$u8192.pfx')

# 16 KB keys
verify-file ($dataPath + '\pfx\failing\nt5$wmmc$pnullterm$ere$c3$u16384.pfx')

# trigger for CA file with issuer string = subject string. Note that certtool currently has this bug
# so this "passes" because we can't see the bug
verify-file ($datapath + '\pfx\spoofpfx$ppassword.pfx')


cmdd certtool verifyCertfile $datapath\pfx\passwd.pfx


function verify-file([string] $filename)
{
  cmdd certtool installcertfile $filename
  cmdd certtool verifycertfile $filename
  cmdd certtool DeleteCertFile $filename
}

# x509store playground'

#print-store "ROOT"
#print-store "MY"
# redmond\scyost = S-1-5-21-2127521184-1604012920-1887927527-2018792
print-store "

function print-store([string] $storeName)
{
  $x = new-object System.Security.Cryptography.X509Certificates.X509Store($storeName)
  $x.open("ReadOnly")
  $x.certificates
  $x.close()
}

# DefaultAppAccount
cmdd walkcertstore S-1-5-21-2702878673-795188819-444038987-1531\MY
cmdd certtool dc -storename my -username S-1-5-21-2702878673-795188819-444038987-1531
# WPCommsServices
cmdd walkcertstore S-1-5-21-2702878673-795188819-444038987-1538\MY
cmdd certtool dc -storename my -username S-1-5-21-2702878673-795188819-444038987-1538
# fails
cmdd certtool dc -storename my -username S-1-5-21-2702878673-795188819-444038987-1538000

# missing X509Store(IntPtr) look at 23120