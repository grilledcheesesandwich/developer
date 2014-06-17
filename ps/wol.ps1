param([string]$MacAddress)

function Send-Packet([string]$MacAddress, [int]$Packets)
{
    <#
    .SYNOPSIS
    Sends a number of magic packets using UDP broadcast.
 
    .DESCRIPTION
    Send-Packet sends a specified number of magic packets to a MAC address in order to wake up the machine.  
 
    .PARAMETER MacAddress
    The MAC address of the machine to wake up.
 
    .PARAMETER
    The number of packets to send.
    #>
 
    try
    {
        $Broadcast = ([System.Net.IPAddress]::Broadcast)
 
        ## Create UDP client instance
        $UdpClient = New-Object Net.Sockets.UdpClient
 
        ## Create IP endpoints for each port
        $IPEndPoint1 = New-Object Net.IPEndPoint $Broadcast, 0
        $IPEndPoint2 = New-Object Net.IPEndPoint $Broadcast, 7
        $IPEndPoint3 = New-Object Net.IPEndPoint $Broadcast, 9
 
        ## Construct physical address instance for the MAC address of the machine (string to byte array)
        $MAC = [Net.NetworkInformation.PhysicalAddress]::Parse($MacAddress)
 
        ## Construct the Magic Packet frame
        $Frame = [byte[]]@(255,255,255, 255,255,255);
        $Frame += ($MAC.GetAddressBytes()*16)
 
        ## Broadcast UDP packets to the IP endpoints of the machine
        for($i = 0; $i -lt $Packets; $i++) {
            $UdpClient.Send($Frame, $Frame.Length, $IPEndPoint1) | Out-Null
            $UdpClient.Send($Frame, $Frame.Length, $IPEndPoint2) | Out-Null
            $UdpClient.Send($Frame, $Frame.Length, $IPEndPoint3) | Out-Null
            sleep 1;
        }
    }
    catch
    {
        $Error | Write-Error;
    }
}

Send-Packet $MacAddress.Replace("-", "").Replace(":", "") 5