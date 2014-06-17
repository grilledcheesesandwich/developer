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

    using Microsoft.RemoteToolSdk.PluginComponents;
    using RemoteCertificateViewer.Data;
    using RemoteCertificateViewer.View;

    /// Plugin object for RemoteCertificateViewer.
    /// The full name of the object in this code is RemoteCertificateViewer.CertificateViewer.
    /// That is specified in the RemoteCertificateViewer.cebundleinfo file.
    public class CertificateViewer : PluginComponent
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
        private string guidTopLevelNode = "ab6602fc-e128-4fd9-b4c2-f48e4c1ef8d9";

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
        public CertificateViewer(
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
                    this.guidChildNode1);

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
            var certsNode = new PluginNode(
                    "Certificates",
                    this.guidChildNode1,
                    this.guidTopLevelNode,
                    this.data,
                    this.view,
                    this,
                    BuiltInIcon.NodeData);
            AddNode(certsNode);
            certsNode.Select();
        }
    }
}
