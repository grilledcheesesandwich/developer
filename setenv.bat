@echo off

set _DEVDIR=%SDXROOT%\developr\%USERNAME%
set _DOCDIR=e:\Documents
set SDFORMEDITOR=%_DEVDIR%\tools\sdforms\sdforms.exe
set SLIC_DESCRIPTION_TEMPLATE_FILE_PATH=\\winphonelabs\securestorage\apollo\Project\OSPlat\MUX\checkins\slictemplate.txt

set PATH=%PATH%;e:\shared\tools;%_DEVDIR%\tools;%_DEVDIR%\ps;%_DEVDIR%\tools\sdh;%_DOCDIR%\Apps\SysInternals;%_DOCDIR%\Apps\ACLSecurity

set _SECDEVDIR=%SDXROOT%\developr\Security
set PATH=%PATH%;%_SECDEVDIR%;%_SECDEVDIR%\ps

REM set BUILD_DEVELOPER_SOURCE=1
REM set ENABLE_OPTIMIZER=0

REM ***** Enable Code Coverage *****
REM set IMGCODECOVERAGE=1
REM set IMGKCOVER=1

REM *****Change merge tool *****
REM set SDMERGE=%_DEVDIR%\tools\3waymerge.bat 
set SDMERGE="C:\Program Files (x86)\Beyond Compare 3\BCompare.exe"

PROMPT $M$P$_$G
pushd %SDXROOT%