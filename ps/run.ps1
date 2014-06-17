build
if ($?)
{
    put-device "$env:BINARY_ROOT\bin\test\common\$env:_TGTCPU\$env:BUILDTYPE\sectool.exe" c:\test\
    exec-device -output c:\test\sectool.exe "$args"
}