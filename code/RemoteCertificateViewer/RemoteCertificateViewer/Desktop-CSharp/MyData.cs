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

namespace RemoteCertificateViewer.Data
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

    using Microsoft.RemoteToolSdk.PluginComponents;
    using RemoteCertificateViewer.View;

    /// <summary>
    /// The device side app expects a command with a value of 1 sent to it,
    /// where it will then fill the return packet with some dummy data.
    /// </summary>
    public class MyData : PluginData
    {
        /// <summary>
        /// Hierarchy of certs
        /// </summary>
        public Stores Stores { get; set; }        

        /// <summary>
        /// Constructor: Build the empty string array
        /// </summary>
        /// <param name="host">Plugin owning this data</param>
        /// <param name="guid">Guid of the node owning this data</param>
        public MyData(
            PluginComponent host,
            string guid)
            : base(host, guid)
        {
            this.InitDataAtViewTime = false;
            this.Stores = new Stores();
        }

        /// <summary>
        /// Store data items coming in from the serializer
        /// </summary>
        /// <param name="description">Description of the data item</param>
        /// <param name="value">Value of the data item</param>
        protected override void OnAddVirtualDataItem(
            string description,
            string value)
        {
            //this.strings.Add(value);
        }

        /// <summary>
        /// Retrieve data from the device and store it in the data items.
        /// </summary>
        protected override void OnGetData()
        {
            CommandPacket sendCommand = new CommandPacket();

            // Populate the command object (command value of 1)
            sendCommand.CommandId = 1;

            // Process the command
            ProcessCommandExData pcexData = new ProcessCommandExData(sendCommand, this);
            pcexData.CommandReceived += new EventHandler(pcexData_CommandReceived);

            CommandTransport.ProcessCommandEx(pcexData);
        }

        /// <summary>
        /// Add a new cert
        /// </summary>
        public void AddCert(string location, string name, string path)
        {
            X509Certificate2 cert = new X509Certificate2(path);

            CommandPacket sendCommand = new CommandPacket();

            sendCommand.CommandId = 2;
            sendCommand.AddParameterString(location);
            sendCommand.AddParameterString(name);
            sendCommand.AddParameterBytes(cert.RawData);

            CommandTransport.SendCommand(sendCommand);

            GetData();
        }

        /// <summary>
        /// Remove a cert
        /// </summary>
        public void RemoveCert(string location, string name, X509Certificate2 cert)
        {
            CommandPacket sendCommand = new CommandPacket();

            sendCommand.CommandId = 3;
            sendCommand.AddParameterString(location);
            sendCommand.AddParameterString(name);
            sendCommand.AddParameterBytes(cert.RawData);

            CommandTransport.SendCommand(sendCommand);

            GetData();
        }

        /// <summary>
        /// Render this object's data items in a generic fashion.
        /// </summary>
        /// <param name="dataAcceptor">Data acceptor to render data items to
        /// </param>
        protected override void OnRenderGeneric(GenericDataAcceptor dataAcceptor)
        {
            foreach (var store in Stores.CurrentUser)
            {
                foreach (var cert in store.Certificates)
	            {
                    string category = String.Format("CurrentUser\\{0}", store.Name);
                    dataAcceptor.AddItem(
                        category,
                        cert.Subject,
                        cert.Thumbprint);
                }
            }

            foreach (var store in Stores.LocalMachine)
            {
                foreach (var cert in store.Certificates)
                {
                    string category = String.Format("LocalMachine\\{0}", store.Name);
                    dataAcceptor.AddItem(
                        category,
                        cert.Subject,
                        cert.Thumbprint);
                }
            }
        }
        
        /// <summary>
        /// Retrieve data from the received command and update the UI
        /// </summary>
        /// <param name="sender">Origin of event</param>
        /// <param name="eventArgs">Event arguments</param>
        private void pcexData_CommandReceived(object sender, EventArgs eventArgs)
        {
            // The device side app builds and returns a packet

            // Take these values and convert to strings and place
            // in our array.
            ProcessCommandExData pcexData = (ProcessCommandExData)sender;
            CommandPacket receivedCommand = pcexData.CommandOut;

            try
            {
                this.Stores = new Stores();
                for (int l = 0; l < 2; l++)
                {
                    List<Store> location = this.Stores.CurrentUser;
                    string loc = receivedCommand.GetParameterString();
                    if (loc == "LocalMachine")
                    {
                        location = this.Stores.LocalMachine;
                    }
                   
                    uint storesCount = receivedCommand.GetParameterDWORD();
                    for (int i = 0; i < storesCount; i++)
                    {
                        string name = receivedCommand.GetParameterString();
                        Store store = new Store(name);

                        uint certsCount = receivedCommand.GetParameterDWORD();
                        for (int j = 0; j < certsCount; j++)
                        {
                            var bytes = receivedCommand.GetParameterBytes();
                            X509Certificate2 cert = new X509Certificate2(bytes);
                            store.Certificates.Add(cert);
                        }
                        location.Add(store);
                    }
                }
            }
            catch
            {
            }

            // By setting Initialized to true, the view panel(s) hooked up
            // to this data object will refresh.
            this.Initialized = true;

            // Unregister from the pcexData event handler so that the object
            // can be garbage collected
            pcexData.CommandReceived -= new EventHandler(pcexData_CommandReceived);
        }
    }
}
