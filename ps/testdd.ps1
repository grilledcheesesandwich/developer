param([string[]]$IDs)

$wtq = @"
<WTTData>
  <Component SetName="Explorers" GroupName="JobExplorer">
    <ComponentData Name="JobExplorer" Version="2.2.2098.0">
      <ComponentData Name="HorzSplitter" Version="2.2.2098.0">
        <Data Position="185" />
      </ComponentData>
      <ComponentData Name="VertSplitter" Version="2.2.2098.0">
        <Data Position="192" />
      </ComponentData>
      <ComponentData Name="TabbedObjectTreeControl" Version="2.2.2098.0" Visible="True">
        <Data SelectedTreeNodePath="$\OS Platform\MUX\Device Update\Agent" CurrentTab="0" />
      </ComponentData>
      <ComponentData Name="ConfigurableListControl" Version="2.2.2098.0">
        <Column Name="" Width="32" SortIndex="0" SortOrder="None" ColumnOrder="0" Default="True" />
        <Column Name="Id" Width="70" SortIndex="-1" SortOrder="None" ColumnOrder="1" Default="True" />
        <Column Name="Name" Width="210" SortIndex="-1" SortOrder="None" ColumnOrder="2" Default="True" />
        <Column Name="AssignedToAlias" Width="80" SortIndex="-1" SortOrder="None" ColumnOrder="3" Default="True" />
        <Column Name="FullPath" Width="190" SortIndex="-1" SortOrder="None" ColumnOrder="4" Default="True" />
        <Column Name="LastUpdatedTime" Width="150" SortIndex="1" SortOrder="Descending" ColumnOrder="5" Default="True" />
      </ComponentData>
      <ComponentData Name="QueryBuilder" Version="2.2.2098.0" Visible="True">
        <ObjectQueryBuilder ControlType="Microsoft.DistributedAutomation.UI.QueryBuilder" ControlAssembly="Microsoft.WTT.UI.Controls.ObjectControls" Assembly="WTTOMJobs" Type="Microsoft.DistributedAutomation.Jobs.Job" Title="Job Query">
          <Expression Index="1" Static="True" AttachWith="" Mapping="DataStore" Field="DataStore" Operator="Equals" OperatorDisplayName="Equals" Value="WindowsPhone_Blue" />
		  {0}
        </ObjectQueryBuilder>
      </ComponentData>
      <ComponentData Name="ToolBar" Version="2.2.2098.0">
        <Data DefinitionController="WindowsPhone_Blue" HideQueryBuilder="True" HideHierarchy="True" MaxItemsToDisplay="500" />
      </ComponentData>
    </ComponentData>
  </Component>
  <ActiveComponent SetName="Explorers" GroupName="JobExplorer" />
</WTTData>
"@

$argsHash = @{}
for ($i = 0; $i -lt $args.Length; $i+=2)
{
    $argsHash[$args[$i].Trim('-')] = $args[$i + 1]
}

$idString = '<Expression AttachWith="Or" Mapping="Id" Field="ID" Operator="Equals" OperatorDisplayName="Equals" Value="{0}" />'
#$splitIDs = $IDs.Split(';')
$splitIDs = $IDs
$extraIDsString = [String]::Empty
foreach ($id in $splitIDs)
{
	$extraIDsString += [String]::Format($idString, $id)
}
[String]::Format($wtq, $extraIDsString) | set-content $env:temp\job.wtq

testd -WtqFileName $env:temp\job.wtq @argsHash
del $env:temp\job.wtq