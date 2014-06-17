//-----------------------------------------------------------------------------
// <copyright file='MyData.cs' company='Microsoft'>
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
// Use of this source code is subject to the terms of your Microsoft Windows CE
// Source Alliance Program license form.  If you did not accept the terms of
// such a license, you are not authorized to use this source code.
// </copyright>
// <summary>
// Data access class
// </summary>
//-----------------------------------------------------------------------------

namespace PushXmlPlugin.Data
{
    using System;
    using System.Collections;

    using Microsoft.RemoteToolSdk.PluginComponents;
    using PushXmlPlugin.View;

    /// <summary>
    /// The device side app expects a command with a value of 1 sent to it,
    /// where it will then fill the return packet with some dummy data.
    /// </summary>
    public class MyData : PluginData
    {
        /// <summary>
        /// Array of strings to represent the data.
        /// </summary>
        private ArrayList strings;

        private MyView m_view;

        /// <summary>
        /// Constructor: Build the empty string array
        /// </summary>
        /// <param name="host">Plugin owning this data</param>
        /// <param name="guid">Guid of the node owning this data</param>
        public MyData(
            PluginComponent host,
            string guid,
            ref MyView view
            )
            : base(host, guid)
        {
            m_view = view;
            this.InitDataAtViewTime = false;
            this.strings = new ArrayList();
        }

        /// <summary>
        /// Returns the array of strings representing the data
        /// </summary>
        public ArrayList Strings
        {
            get { return this.strings; }
            set { this.strings = value; }
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
            this.strings.Add(value);
        }

        /// <summary>
        /// Retrieve data from the device and store it in the data items.
        /// </summary>
        protected override void OnGetData()
        {
            // By setting Initialized to true, the view panel(s) hooked up
            // to this data object will refresh.
            this.Initialized = true;
        }

        /// <summary>
        /// Render this object's data items in a generic fashion.
        /// </summary>
        /// <param name="dataAcceptor">Data acceptor to render data items to
        /// </param>
        protected override void OnRenderGeneric(GenericDataAcceptor dataAcceptor)
        {
            //string category = "My category";

            //for (int index = 0; index < this.strings.Count; index++)
            //{
            //    dataAcceptor.AddItem(
            //        category,
            //        "String #" + index.ToString(),
            //        this.strings[index].ToString());
            //}
        }

        protected override void OnCopyToClipboard()
        {
            
        }

        /// <summary>
        /// Send Xml to device
        /// </summary>
        /// <param name="xml">Data to set</param>
        /// <param name="isCfgMgr1">ConfigManager1 xml</param>
        /// <param name="role">The rolemask you want to run CfgMgr with</param>
        public CommandInfo SendXmlToDevice(string xml, bool isCfgMgr1, uint role)
        {
            CommandPacket commandPacket = new CommandPacket();
            commandPacket.CommandId = 1;

            // 1 = ConfigManager1, 2 = ConfigManager2
            commandPacket.AddParameterDWORD(isCfgMgr1 ? 1u : 2u);
            commandPacket.AddParameterString(xml);
            commandPacket.AddParameterDWORD(role);

            // Process the command
            CommandPacket receivedCommand =
                Host.CommandTransport.ProcessCommand(commandPacket, this);

            CommandInfo command = new CommandInfo();

            if (2 == receivedCommand.CommandId) // There was an exception
            {
                command.ErrorMessage = receivedCommand.GetParameterString();
            }
            else if (1 == receivedCommand.Count)
            {
                command.ErrorMessage = "No output wap doc received";
                command.ErrorCode = receivedCommand.GetParameterDWORD();
            }
            else if (2 == receivedCommand.Count)
            {
                command.XmlDoc = receivedCommand.GetParameterString();
                command.ErrorCode = receivedCommand.GetParameterDWORD();
            }
            else
            {                
                command.ErrorMessage = "No parameters received";
            }

            return command;
        }        
    }

    /// <summary>
    /// Object holding the returned ErrorCode, ErrorMessage and response Xml
    /// </summary>
    public class CommandInfo
    {
        private string m_errorMessage;
        private uint m_errorCode;
        private string m_xmlDoc;

        /// <summary>
        /// Holds the ErrorMessage (if there is one)
        /// </summary>
        public string ErrorMessage
        {
            get { return m_errorMessage; }
            set { m_errorMessage = value; }
        }

        /// <summary>
        /// Holds the ErrorCode (if there is one)
        /// </summary>
        public uint ErrorCode
        {
            get { return m_errorCode; }
            set { m_errorCode = value; }
        }

        /// <summary>
        /// Holds the response Xml document (if there is one)
        /// </summary>
        public string XmlDoc
        {
            get { return m_xmlDoc; }
            set { m_xmlDoc = value; }
        }
    }
}
