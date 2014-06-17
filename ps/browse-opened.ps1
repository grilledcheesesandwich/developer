$clientMappings = New-Object "System.Collections.Generic.Dictionary``2[System.String,System.String]"
$opened = @(sdx opened ...)
$client = @(sdx client -o)

foreach ($line in $client)
{
    $trimmed = $line.Trim();
    if ($trimmed.StartsWith("//depot"))
    {
        $tokens = $trimmed.Split(' ');
        $subDir = $tokens[1].Split('/') | select -Skip 3
        $subDir = [String]::Join("\", $subDir)
        $directory = "{0}\{1}" -f $env:_WINPHONEROOT, $subDir
        $clientMappings[$tokens[0].Trim('.').ToLower()] = $directory.Trim('.')
    }
}

$opened = $opened | ? { $_.StartsWith("//depot") }

[void][reflection.assembly]::LoadWithPartialName("System.Windows.Forms")
$form = new-object Windows.Forms.Form
$form.Width = [Windows.Forms.Screen]::PrimaryScreen.WorkingArea.Width;
$form.Text = "Select file to diff"
$form.AutoSize = $true;
$form.Add_Shown({$form.Activate()})

$changelistMappings = @{}
$opened | % {
    foreach ($key in $clientMappings.Keys)
    {
        $lowered = $_.ToLower()
        if ($lowered.StartsWith($key))
        {
            $entry = $lowered.Replace($key, $clientMappings[$key]).Replace('/', '\')
            $latter = $entry.Split('#')[1]
            if ($latter -match "default")
            {
                $changelist = "default"
            }
            else
            {
                $changelist = [Text.RegularExpressions.Regex]::Matches($latter, "\d+")[1].Value
            }
            if (!$changelistMappings.ContainsKey($changelist))
            {
                $changelistMappings[$changelist] = new-object System.Collections.Generic.List[String]
            }
            $changelistMappings[$changelist].Add($entry)
            
            break
        }        
    }
}

$i = $changelistMappings.Keys.Count
$colors = @([Drawing.Color]::Blue, [Drawing.Color]::Green, [Drawing.Color]::Black, [Drawing.Color]::Purple)
$changelistMappings.Keys | % {
    $listBox = new-object Windows.Forms.ListBox
    $listBox.Font = new-object System.Drawing.Font [System.Drawing.FontFamily]::GenericMonospace, 12
    $listBox.Dock="Top"
    $listBox.AutoSize = $true
    $listBox.Add_KeyDown(
    {
        if ($_.KeyCode -eq "Enter")
        {
            if ($this.SelectedItem.Contains("#0 - add"))
            {
                ii $this.SelectedItem.Split('#')[0]
            }
            else
            {
                windiff -L $this.SelectedItem.Split('#')[0]
            }
        }
        if ($_.KeyCode -eq "Escape") { $form.DialogResult = "Cancel"; $form.Close() }
    })
    $listBox.Add_DoubleClick(
    {
        if ($this.SelectedItem.Contains("#0 - add"))
        {
            ii $this.SelectedItem.Split('#')[0]
        }
        else
        {
            windiff -L $this.SelectedItem.Split('#')[0]
        }
    })
    $listBox.TabIndex = --$i
    $listBox.ForeColor = $colors[$i % $colors.Length]
    $listBox.Items.AddRange($changelistMappings[$_])
    $form.controls.add($listBox)
}

# start the editor and wait for it to exit...
# if user pressed Enter, diff selected file
if ($form.ShowDialog() -eq "OK")
{
    
}