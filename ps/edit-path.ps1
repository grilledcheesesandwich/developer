#Get the SYSTEM PATH
$path = $env:path
$splitPath = $path.Split(';') | sort

# start the editor and wait for it to exit...
[void][reflection.assembly]::LoadWithPartialName("System.Windows.Forms")
$form = new-object Windows.Forms.Form
$form.Width = 800
$form.Height = 600
$form.Text = "Edit %Path%"

$textBox = new-object Windows.Forms.RichTextBox
$textBox.Lines = $splitPath
$textBox.Dock="Fill"
$form.controls.add($textBox)

$button = new-object Windows.Forms.Button
$button.text="Done"
$button.Dock="Bottom"
$button.DialogResult = "OK"
$button.add_click({$form.close()})
$form.controls.add($button)

$form.Add_Shown({$form.Activate()})

# if user clicked "Done", write new path to %path% variable
if ($form.ShowDialog() -eq "OK")
{
    $env:path = [String]::Join(';', $textBox.Lines)
    "Path updated"
}