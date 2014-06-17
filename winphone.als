REM useful aliases

REM Navigation aliases
r     pushd %_FLATRELEASEDIR%
x     pushd %_WINPHONEROOT%
sec   pushd %_WINPHONEROOT%\private\winceos\comm\security
s     pushd %_WINPHONEROOT%\private\test\Security
sdcsp pushd %_WINPHONEROOT%\private\shellw\csp\appdisable
d     pushd %_WINPHONEROOT%\developr\%USERNAME%
prov  pushd d:\documents\apps\flashing\provisioning
pri   pushd %_WINPHONEROOT%\private\$1
pub   pushd %_WINPHONEROOT%\public\$1

REM Test Dir navigation aliases
td     pushd %_WINPHONEROOT%\private\test\mobile\automation\tests\DeviceManagement
ts     pushd %_WINPHONEROOT%\private\test\mobile\automation\tests\Security
ad     pushd %_WINPHONEROOT%\private\test\mobile\automation\AreaLibrary\DeviceManagement
as     pushd %_WINPHONEROOT%\private\test\mobile\automation\AreaLibrary\Security

csp    pushd %_WINPHONEROOT%\private\test\mobile\automation\tests\DeviceManagement\CSP\SoftwareDisable
ar     pushd %_WINPHONEROOT%\private\test\mobile\automation\AreaLibrary\DeviceManagementUI\SoftwareDisable
ab     pushd %_WINPHONEROOT%\private\test\mobile\automation\AbstractionLayer\DeviceManagement\SoftwareDisable
pol    pushd %_WINPHONEROOT%\private\test\mobile\automation\AreaLibrary\Security\PolicyRules
acc    pushd %_WINPHONEROOT%\private\test\mobile\automation\AreaLibrary\Security\Accounts

REM Command aliases
REM *DANGEROUS* dd        cd $*$Tsd delete ...$Tcd ..$Trd $*
.         explorer .
n         notepad $*
fsi       findstr /I /S /N $*
fsip      findstr /I /S /N /P $*
cd        cd $* $T dir
w         windiff -L $*
jjdiff    jjpack windiff $*
deltree   rd /Q /S $*
certs     certificates.msc
rev       revoke $* out.xml $T type out.xml
flat      copy $* %_flatreleasedir%
noro      attrib -r $*
setfind   set | findstr /i $*
osd       %_DEVDIR%\OSDesigns\Seven\Seven.pbxml
syncplat  sd online %_WINPHONEROOT%\platform\... $T sd revert %_WINPHONEROOT%\platform\...
listbuilds dir \\binshop\release\seven\mainline $T dir \\build\release\seven\mainline
whereedit for /f %%i in ('where $*') DO notepad %%i
submit    pushd %_WINPHONEROOT% $T submit
lv        copy %_flatreleasedir%\lvperfall.csv d:\shared\temp\ $T d:\shared\temp\lvperfall.csv
ril       del %_flatreleasedir%\rillog.dll
bc        \"%PROGRAMFILES%\Beyond Compare 2\BC2.exe\" $1 $2
sl        sync latest $1 $2 $3 $4
p86       %SystemRoot%\syswow64\WindowsPowerShell\v1.0\powershell.exe

REM Source index aliases
srch      start \\wmsdsearch\search\%_RELEASELABEL%
cip       ci \"$1 AND #FILENAME $2\"
cipedit   for /f %%i in ('ci \"$1 AND #FILENAME $2\" /q') DO notepad \"%%i\"
cidir     ci \"#FILENAME $1\"
ciedit    for /f %%i in ('ci \"#FILENAME $1\" /q') DO notepad \"%%i\"

REM Powershell aliases
ps        powershell $*
lastlog   powershell %_DEVDIR%\tools\powershell_scripts\get-lastlog.ps1
getcert   powershell $(Get-AuthenticodeSignature $*).SignerCertificate.Subject

REM bbPack aliases
pack      bbpack -c $1 -f -o \\undev\shared\packs\$2.cmd
unpack    \\undev\shared\packs\$1.cmd -u $2 $3 $4 $5 $6 $7 $8 $9
listpacks dir \\undev\shared\packs /od
diffpack  \\undev\shared\packs\$1.cmd -w
delpack   del /p \\undev\shared\packs\$1.cmd
mergepack \\undev\shared\packs\$1.cmd -M -f -u $2 $3 $4 $5 $6 $7 $8 $9
