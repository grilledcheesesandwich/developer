//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this sample source code is subject to the terms of the Microsoft
// license agreement under which you licensed this sample source code. If
// you did not accept the terms of the license agreement, you are not
// authorized to use this sample source code. For the terms of the license,
// please see the license agreement between you and Microsoft or, if applicable,
// see the LICENSE.RTF on your install media or the root of your tools installation.
// THE SAMPLE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//

namespace RemoteCertificateViewer
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using System.Collections.Generic;

    using Microsoft.RemoteToolSdk.DeviceComponents;

    /// <summary>
    /// Main program class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Transport command object
        /// </summary>
        private static DeviceCommandTransport commandTransport;

        /// <summary>
        /// Main entry point into the program
        /// </summary>
        /// <param name="args">Command line arguments</param>
        public static void Main(string[] args)
        {
            commandTransport = new DeviceCommandTransport(args[0]);

            commandTransport.CommandPacketReceived +=
                new CommandPacketEventHandler(
                    CommandTransport_CommandPacketReceived);

            // This starts the command handler as a synchronous command
            // handler. The desktop side code initiates a command packet,
            // to which the device side responds in some way.
            commandTransport.StartCommandHandlerSync();
        }

        /// <summary>
        /// Received command packet
        /// </summary>
        /// <param name="sender">Origin of event</param>
        /// <param name="e">Event arguments</param>
        private static void CommandTransport_CommandPacketReceived(
            object sender,
            CommandPacketEventArgs e)
        {
            switch (e.CommandPacketIn.CommandId)
            {
                // Querying cert stores
                case 1:

                    CommandPacket commandPacket = new CommandPacket();
                    commandPacket.CommandId = 1;

                    var hklmStores = new List<X509Store>();
                    var hkcuStores = new List<X509Store>();

                    var hStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
                    hklmStores.Add(hStore);
                    //hStore = new X509Store(StoreName.CertificateAuthority, StoreLocation.CurrentUser);    // broken
                    //hkcuStores.Add(hStore);

                    hStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                    hklmStores.Add(hStore);
                    hStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                    hkcuStores.Add(hStore);

                    hStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
                    hklmStores.Add(hStore);
                    //hStore = new X509Store(StoreName.Root, StoreLocation.CurrentUser);    // broken
                    //hkcuStores.Add(hStore);

                    hStore = new X509Store("Code Integrity", StoreLocation.LocalMachine);
                    hklmStores.Add(hStore);

                    hklmStores.ForEach(store => store.Open(OpenFlags.ReadOnly));
                    hkcuStores.ForEach(store => store.Open(OpenFlags.ReadOnly));

                    commandPacket.AddParameterString(StoreLocation.LocalMachine.ToString());
                    commandPacket.AddParameterDWORD((uint)hklmStores.Count);
                    foreach (var store in hklmStores)
	                {
                        commandPacket.AddParameterString(store.Name);
                        commandPacket.AddParameterDWORD((uint)store.Certificates.Count);
                        foreach (var cert in store.Certificates)
                        {
                            commandPacket.AddParameterBytes(cert.GetRawCertData());
                        }
	                }

                    commandPacket.AddParameterString(StoreLocation.CurrentUser.ToString());
                    commandPacket.AddParameterDWORD((uint)hkcuStores.Count);
                    foreach (var store in hkcuStores)
                    {
                        commandPacket.AddParameterString(store.Name);
                        commandPacket.AddParameterDWORD((uint)store.Certificates.Count);
                        foreach (var cert in store.Certificates)
                        {
                            commandPacket.AddParameterBytes(cert.GetRawCertData());
                        }
                    }
                    
                    e.CommandPacketOut = commandPacket;

                    hklmStores.ForEach(store => store.Close());
                    hkcuStores.ForEach(store => store.Close());

                    break;

                // Adding a cert
                case 2:
                    string location = e.CommandPacketIn.GetParameterString();
                    string name = e.CommandPacketIn.GetParameterString();
                    byte[] bytes = e.CommandPacketIn.GetParameterBytes();

                    StoreLocation storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), location, true);
                    X509Store addStore = new X509Store(name, storeLocation);
                    addStore.Open(OpenFlags.ReadWrite);
                    var addCert = new X509Certificate2(bytes);
                    addStore.Add(addCert);
                    addStore.Close();

                    break;

                // Removing a cert
                case 3:
                    location = e.CommandPacketIn.GetParameterString();
                    name = e.CommandPacketIn.GetParameterString();
                    bytes = e.CommandPacketIn.GetParameterBytes();
                    var removeCert = new X509Certificate2(bytes);

                    Console.WriteLine(@"Removing {0} from {1}\{2}", removeCert.GetName(), location, name);

                    storeLocation = (StoreLocation)Enum.Parse(typeof(StoreLocation), location, true);
                    X509Store removeStore = new X509Store(name, storeLocation);
                    removeStore.Open(OpenFlags.ReadWrite);
                    removeStore.Remove(removeCert);
                    removeStore.Close();

                    break;
            }
        }
    }
}
