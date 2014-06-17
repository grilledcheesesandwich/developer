param([string] $path = "$\CorePlat\CoreOS\CSI\SecurityModel\Device")
$jobs = .\get-wttjobs.ps1 -path $path
foreach ($job in $jobs)
{
    "`n`n{0} : {1}" -f $job.Id, $job.Name
    $testPackageParameter = $job.CommonContext.ParameterCollection | ? { $_.Name -eq "system_TestPackages" }
    $parameterValue = $testPackageParameter.ParameterVal
    if ($parameterValue -eq $null) { continue }
    $packages = $parameterValue.Split(";")
    $packages | % {
        $package = dir -recurse $env:BINARY_ROOT\prebuilt\test -filter ($_ + ".dep.xml")
        if ($package -eq $null)
        {        
            $package = dir -recurse $env:BINARY_ROOT\prebuilt\unittest -filter ($_ + ".dep.xml")
        }
        if ($package -eq $null) { continue }
        $xml = [xml](gc $package.FullName)
        $packageDependencies = $xml | Select-Xml '//Package' | % { dir ("$env:BINARY_ROOT\prebuilt\" + $_.Node.Name) }
        
        $size = ($packageDependencies | Measure-Object -Property Length -Sum).Sum / 1024 / 1024
        "{0} : {1:0.00} MB" -f $_, $size
        $packageDependencies | Select -Property FullName

        #if ($size -gt 100)
        #{
            #$job.Name
            #$packageDependencies | ? { $_.Length -gt 40000000 } | % { $_.Name; $_.Length / 1000000 }
            #Write-Host -ForegroundColor Red $size
        #}
    }
}