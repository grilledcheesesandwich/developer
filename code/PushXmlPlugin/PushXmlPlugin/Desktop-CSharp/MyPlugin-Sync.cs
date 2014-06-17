//-----------------------------------------------------------------------------
// <copyright file='MyPlugin-Sync.cs' company='Microsoft'>
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
// Use of this source code is subject to the terms of your Microsoft Windows CE
// Source Alliance Program license form.  If you did not accept the terms of
// such a license, you are not authorized to use this source code.
// </copyright>
// <summary>
// Plugin class
// </summary>
//-----------------------------------------------------------------------------

namespace PushXmlPlugin
{
    using System;

    using Microsoft.RemoteToolSdk.PluginComponents;
    using PushXmlPlugin.Data;
    using PushXmlPlugin.View;

    /// Plugin object for PushXmlPlugin.
    /// The full name of the object in this code is PushXmlPlugin.PushXml.
    /// That is specified in the PushXmlPlugin.cebundleinfo file.
    public class PushXml : PluginComponent
    {
        // A plugin has one or more nodes in a TreeView control that lives on
        // the left panel of the Remote Tools Shell. Each node has a GUID so that
        // a parent-child relationship can be defined. If two different plugins wish to
        // share a parent node, they would then both need to use the same GUID to
        // define the parent.
        //
        // In this example, the tree will look like this:
        //
        //  <root>
        //  My Plugin
        //      Device Time

        /// <summary>
        /// Guid for top level tree node
        /// </summary>
        private string guidTopLevelNode = "8c119b55-8786-4bdb-9f6a-f3ac4f053634";

        /// <summary>
        /// Guid for first child node
        /// </summary>
        private string guidChildNode1 = "guidChildNode1";

        // These are object(s) that contain the actual data uploaded from the device.
        // The data classes must implement a OnGetData() method, which is the function that
        // is responsible for filling these object(s) with data.
        //
        // The user is free to implment these data objects however they want, as long
        // as they are based on the PluginData class. That class provides a
        // OnGetData method to override, as well as other data-centric functions.

        /// <summary>
        /// Plugin data object
        /// </summary>
        private MyData data;

        // These are objects that represent a view upon a data object. One data object can
        // have as many views as it would like. Each view is coupled to one data type, unless
        // you add some sort of standard interface to all of your data objects (such as the
        // RenderGeneric method).
        //
        // A view object is automatically "hooked up" to a data object if you specify them
        // as a pair in the PluginNode constrcutor (see OnInit, below). If you are doing some
        // implementation that requires you to hook up views and data objects manually, use the
        // AssociateWithData method in the view.

        /// <summary>
        /// Plugin view object
        /// </summary>
        private MyView view;

        /// <summary>
        /// Constructor for this object. You can set some attributes here.
        /// </summary>
        /// <param name="deviceGuid">Guid for device</param>
        /// <param name="pluginGuid">Guid for plugin</param>
        public PushXml(
            string deviceGuid,
            string pluginGuid)
            : base(deviceGuid, pluginGuid)
        {

        }

        /// <summary>
        /// Called by the Remote Tools Framework at load-time for the plugin.
        /// You need to define the node structure and create your data objects here.
        /// </summary>
        protected override void OnInit()
        {
            // Buld our data objects and add to the built-in array.
            // By adding to the built-in array, the Remote Tools Framework
            // can enumerate them for serialization.
            this.data =
                new MyData(
                    this,
                    this.guidChildNode1,
                    ref this.view);

            AddData(this.data);



            // Build a view object
            this.view = new MyView(this.data);

            // If the plugin is running "standalone", that means that it does not need
            // to share the TreeView control in the shell with anyone else. It would
            // be redundant to add a top node to identify the plugin (the one with a
            // hammer and wrench).
            if (this.Standalone)
            {
                this.guidTopLevelNode = null;
            }
            else
            {
                // Otherwise... this plugin is sharing the TreeView with other plugins,
                // and therefore should add a top level node to idenifty the plugin.
                //
                // This will add a top level node by setting the guid for the parent to null.
                // There is no view panel associated with this node.
                AddNode(
                    new PluginNode(
                        Title,
                        this.guidTopLevelNode,
                        null,
                        null,
                        null,
                        this,
                        BuiltInIcon.Plugin));
            }

            // Add the actual node for the data. Specify the guid
            // of the "My Plugin" node for the parent, and specify
            // the data and view.
            AddNode(
                new PluginNode(
                    "PushXML",
                    this.guidChildNode1,
                    this.guidTopLevelNode,
                    this.data,
                    this.view,
                    this,
                    BuiltInIcon.NodeData));
        }
    }
}
