param([switch] $disable)

if (!$disable)
{
    "** enabling .net 4 support in PS"
    reg add hklm\software\microsoft\.netframework /v OnlyUseLatestCLR /t REG_DWORD /d 1 /f
    reg add hklm\software\wow6432node\microsoft\.netframework /v OnlyUseLatestCLR /t REG_DWORD /d 1 /f
}
else
{
    "** disabling .net 4 support in PS"
    reg delete hklm\software\microsoft\.netframework /v OnlyUseLatestCLR /f
    reg delete hklm\software\wow6432node\microsoft\.netframework /v OnlyUseLatestCLR /f
}
