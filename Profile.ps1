ri alias:r
ri alias:cd
ri alias:set
ri alias:pushd
ri alias:popd
ri alias:cp
ri alias:ls

$env:PATH += ";$PSScriptRoot"
$env:PATH += ";$PSScriptRoot\tools"
$env:PATH += ";$PSScriptRoot\ps"
$env:DEVDIR = "$PSScriptRoot"

# Navigation functions
function doc   { pushd $env:_DOCDIR }
function dev   { pushd $env:DEVDIR }

# Command functions
function e          { ii . }
function ..         { pushd ..; gci }
function ...        { pushd ..\..; gci }
function ....       { pushd ..\..\..; gci }
function .....      { pushd ..\..\..\..; gci }
set-alias   n       "notepad"
function fsip       { findstr /I /S /N /P $args }
function cd         { chdir $args[0]; gci }
function w          { windiff -L $args }
function o          { odd -lo $args }
function deltree    { del $args -recurse -force }
set-alias   certs   "certificates.msc"
function ccd        { $result = ccd3.exe $args; if ($LASTEXITCODE -eq 0) { pushd $result; gci } else { $result } }
function noro       { attrib -r $args }
function setfind    { gci env: | findstr /i $args | sort }
function whereedit  { where.exe $args | % { notepad $_ } }
function bc         { &"$env:ProgramFiles\Beyond~1\BC2.exe" $args }
function delr       { gci -filter $args[0] -recurse | del -verbose }
function getcert    { $(Get-AuthenticodeSignature $args).SignerCertificate }
function viewcert   { $cerPath = "$env:TEMP\cer.cer"; ($(Get-AuthenticodeSignature $args).SignerCertificate).RawData | set-content $cerPath -encoding byte; ii $cerPath }
function ise        { &"$PSHOME\powershell_ise.exe" $args }
function pse        { &"C:\Program Files (x86)\PowerGUI\ScriptEditor_x86.exe" $args }
function dirr       { Get-ChildItem -filter $args[0] -recurse | select FullName }
function dirdir     { Get-ChildItem -filter $args[0] | where { $_.PSIsContainer } | select FullName }
function copyconsole { powershell -sta -command copy-console }
set-alias ss        select-string
function cp         { $pwd.Path | setclip }
function crp        { [String]::Join("\", ($pwd.Path.Split("\") | select -skip 2)) | setclip }
function share      { $fileName = (new-object System.IO.FileInfo $args).Name; $target = "\\$env:COMPUTERNAME\shared\temp\$fileName"; copy $args $target; $target | setclip }
function mdcd		{ md $args; cd $args[0] }
function rgx		{ [regex]::Matches($args[1], $args[0]) | % { $_.Value } }
function gloc       { gci -r * -I *.cpp,*.h,*.c,*.cs,*.cxx,*.hxx | gc | measure -Line -Word -Character | fl }
function tmp        { pushd $env:TEMP }
function attach     { "SELECT VDISK FILE=`"$(resolve-path $args)`"`r`nATTACH VDISK" | diskpart }
function detach     { "SELECT VDISK FILE=`"$(resolve-path $args)`"`r`nDETACH VDISK" | diskpart }
filter prop			([string]$property) { $_.$property }
filter split        { $_.Split("`n").Trim() }
function hex        { "0x{0:X}" -f [long]$args[0] }
function wh         { $result = where.exe $args; $result; @($result)[0] | setclip }
function copylast   { Get-History -Count 1 | prop CommandLine | setclip }
function nodejs     { C:\Windows\SysWOW64\cmd.exe /k "C:\Program Files (x86)\nodejs\nodevars.bat" }

# navigation stack functions
function pushd     { Push-Location -StackName back $args[0] }
function popd      { Push-Location -StackName forward $pwd; Pop-Location -StackName back }
Set-Alias p        "popd"
function f         { Push-Location -StackName back $pwd; Pop-Location -StackName forward }

# git stuff
function gs { git status $args }
function ga { git add $args }
function gb { git branch $args }
function gcom { git commit $args }
function gd { if ($args.Count -eq 0) { git dt } else { git difftool $args } }
function go { git checkout $args}
function hist { git hist $args }

function Get-Thumbprint([string]$BinaryPath, [string]$Algorithm = "SHA256")
{
    $bytes = $(Get-AuthenticodeSignature $BinaryPath).SignerCertificate.RawData
    $hasher = new-object "System.Security.Cryptography.$($Algorithm)Managed"
    $hasher.ComputeHash($bytes) | % { write-host -nonewline ("{0:X2}" -f $_) };""
}

function Get-ClipboardText {
        $command = {
                add-type -an system.windows.forms
                [System.Windows.Forms.Clipboard]::GetText()
        }
        powershell -sta -noprofile -command $command
}

function set
{
    $strings = $args[0].Split('=')
    $var = $strings[0]
    $value = $strings[1]
    Set-Content -Path env:\$var -Value $value
}

Function Pairwise {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory=$True,ValueFromPipeline=$True,ValueFromPipelinebyPropertyName=$True)] $item
    )
    
    Begin
    {
        $i = 0;
        $list = @();
    }
    
    PROCESS {        
        $list = $list + $item
        
        $item | % {
            foreach ($li in ($list | select -first ($i++)))
            {
                new-object psobject -property @{left=$_;right=$li}
            }
        }
    }
}

# powershell specific functions
function search-help
{
    $pshelp = "$pshome\en-US\about_*.txt", "$pshome\en-US\*dll-help.xml"
    select-string -path $pshelp -pattern $args[0]
}

function get-guihelp 
{
    $profilepath = "$env:_DOCDIR\Apps\Powershell\PowerShellHelp\powershell.chm"

    if ($args[0].contains("about_"))

        {$a = "HH.EXE mk:@MSITStore:" + $profilepath + "::/about/" + $args[0] + ".help.htm"
         Invoke-Expression $a}

    elseif ($args[0].contains("-"))

        {$a = "HH.EXE mk:@MSITStore:" + $profilepath + "::/cmdlets/" + $args[0] + ".htm"
         Invoke-Expression $a}

    else

        {if ($args[0].contains(" "))    

            {$b = $args[0] -replace(" ","")
             $a = "HH.EXE mk:@MSITStore:" + $profilepath + "::/vbscript/" + $b + ".htm"
             Invoke-Expression $a
            }

        else
   
            {$b = $args[0] 
             $a = "HH.EXE mk:@MSITStore:" + $profilepath + "::/vbscript/" + $b + ".htm"
             Invoke-Expression $a
             }

$a
        }

} 
Set-Alias gg Get-GUIHelp

# dot-source search helpers
# . researchHelpers.ps1

if ($env:_NTDEVELOPER -eq $null)
{
    Write-Host -ForegroundColor Green "Not an NT build window, loading Arcas"    
    
    $env:path += ";C:\Program Files (x86)\Git\bin"

    # Make sure to set custom $prompt after loading Arcas
    if (test-path 'D:\arcadia\ArcadiaEngSys\Arcas\Arcas.ps1')
    {
        . 'D:\arcadia\ArcadiaEngSys\Arcas\Arcas.ps1'
    }
}
else
{   
    Write-Host -ForegroundColor Green "NT build window, loading NT profile"
    . "$env:_NTDEVELOPER\profile.ps1"
}

function global:prompt
{
    $historyItem = Get-History -Count 1
    if ($historyItem)
    {
        $lastCommandDuration = $historyItem.EndExecutionTime - $historyItem.StartExecutionTime
 
        if ($lastCommandDuration.TotalSeconds -gt 10)
        {
            [Console]::Beep(500,200)
        
            $seconds = ""
            $mins = ""
            $hours = ""
            $totalHours = $lastCommandDuration.Days * 24 + $lastCommandDuration.Hours
 
            if ($lastCommandDuration.Seconds)
            {
                $seconds = "$($lastCommandDuration.Seconds)s"
            }
            if ($lastCommandDuration.Minutes)
            {
                $mins = "$($lastCommandDuration.Minutes)m"
            }
            if ($totalHours)
            {
                $hours = "$($totalHours)h"
            }
            Write-Host -ForegroundColor DarkGray "$($historyItem.CommandLine) <=> $hours$mins$seconds [$($historyItem.EndExecutionTime.ToShortTimeString())]"
        }
    }
    
    $matches = [regex]::Matches($historyItem.CommandLine, "[a-zA-Z]+(-[a-zA-Z]+)?")
    if ($matches.Count -gt 0)
    {
        foreach ($match in $matches)
        {
            $alias = gal -def $match.Value
            if ($alias -ne $null)
            {
                $suggestion = "Suggestion: An alias for {0} is `"{1}`"" -f $match.Value, @($alias)[0]
                foreach ($furtherMatch in $alias | select -skip 1)
                {
                    $suggestion += (" or `"{0}`"" -f $furtherMatch)
                }
                Write-Host -ForegroundColor DarkGray $suggestion
            }
        }
    }
    
    if ($devicewd -ne $null)
    {
        # tshell-style prompt
        "DEVICE $devicewd`nPS $(get-location)`n> "
    }
    elseif ($GitPromptSettings -ne $null)
    {
        # Set up a simple prompt, adding the git prompt parts inside git repos
        $realLASTEXITCODE = $LASTEXITCODE

        # Reset color, which can be messed up by Enable-GitColors
        $Host.UI.RawUI.ForegroundColor = $GitPromptSettings.DefaultForegroundColor

        Write-Host("PS " + $pwd.ProviderPath) -nonewline

        Write-VcsStatus

        $global:LASTEXITCODE = $realLASTEXITCODE
        return "`n> "
    }
    else
    {
        # default prompt
        "PS $(get-location)`n> "
    }
}