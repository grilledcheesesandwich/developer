$client = sd info | select-string "(?<=Client name: )\S+" | % { $_.Matches[0].Value }
$changes = sd changes -r -s pending -c $client | ? { $_ -match "(\d{5,})[^']*'([^']*)" } | % { 
    $matches[1]
}
$changes | % {
    # Redirect stderr to stdout
    $opened = sd opened -c $_ 2>&1
    # If the changelist is empty, sd opened will return an error
    if ($opened -is [System.Management.Automation.ErrorRecord])
    {
        sd change -d $_
    }
}
