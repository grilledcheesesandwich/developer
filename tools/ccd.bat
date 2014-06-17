@echo off
set result=
for /F %%L in ('ccd3 %*') do (set result=%%L)
IF "%result%" == "" goto :EOF
pushd %result%&&dir
