//-----------------------------------------------------------------------------
// <copyright file='Sync.cs' company='Microsoft'>
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
// Use of this source code is subject to the terms of your Microsoft Windows CE
// Source Alliance Program license form.  If you did not accept the terms of
// such a license, you are not authorized to use this source code.
// </copyright>
// <summary>
// Synchronous device side class
// </summary>
//-----------------------------------------------------------------------------

namespace PushXmlPlugin
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.RemoteToolSdk.DeviceComponents;

    /// <summary>
    /// Main program class
    /// </summary>
    public class Program
    {
        [DllImport("DMCoreQA.dll")]
        private static extern uint RunConfigManagerCore(String wapDocument, uint flags, uint roleMask, ref IntPtr documentPointer);

        [DllImport("DMCoreQA.dll", SetLastError = true)]
        private static extern int RunOmaDmCommandsCore(String omaDmInDocument,
            ref IntPtr omaDmOutDocumentPointer, uint uMsgID, uint uClientMsgID,
            uint uSessionID, uint uSyncHdrResult, uint uSecRole, String src, String serverId);

        [DllImport("coredll.dll")]
        public static extern uint LocalFree(IntPtr systemString);

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
            CommandPacket commandPacket = new CommandPacket();
            String outputWapDoc;
            uint returnValue = 0;
            IntPtr documentPointer = IntPtr.Zero;

            commandPacket.CommandId = 1;    // No exceptions

            try
            {
                // 1 = ConfigManager1, 2 = ConfigManager2
                switch (e.CommandPacketIn.GetParameterDWORD())
                {
                    // ConfigManager1
                    case 1:
                        returnValue = RunConfigManagerCore(e.CommandPacketIn.GetParameterString(), 1, e.CommandPacketIn.GetParameterDWORD(), ref documentPointer);
                        break;

                    // ConfigManager2
                    case 2:
                        returnValue = (uint)RunOmaDmCommandsCore(e.CommandPacketIn.GetParameterString(), ref documentPointer, 1, 2, 1, 0, e.CommandPacketIn.GetParameterDWORD(), null, null);
                        break;
                }

                // Marshal and cleanup response string
                outputWapDoc = Marshal.PtrToStringUni(documentPointer);
                commandPacket.AddParameterString(outputWapDoc);
            }
            catch (Exception ex)
            {
                commandPacket.AddParameterString("Exception thrown: " + ex.Message);
                commandPacket.CommandId = 2;        // Exception
            }

            LocalFree(documentPointer);
            commandPacket.AddParameterDWORD(returnValue);
            e.CommandPacketOut = commandPacket;
        }
    }
}
