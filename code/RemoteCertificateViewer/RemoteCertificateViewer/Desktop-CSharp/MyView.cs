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

namespace RemoteCertificateViewer.View
{
    using System;
    using System.Windows.Forms;
    using System.Text;
    using System.Security.Cryptography.X509Certificates;
    using System.Linq;

    using Microsoft.RemoteToolSdk.PluginComponents;
    using RemoteCertificateViewer.Data;

    /// <summary>
    /// Plugin view class for My Plugin.
    /// </summary>
    public class MyView : PluginDataView
    {
        private ContextMenuStrip contextMenu;
        private System.ComponentModel.IContainer components;
        private ToolStripMenuItem addCertToolStripMenuItem;
        private OpenFileDialog openFileDialog;
        /// <summary>
        /// List of string data
        /// </summary>
        //private ListBox listboxStrings;
        private TreeView treeviewCerts;

        /// <summary>
        /// Construct a view. This object is created on the primary UI thread.
        /// </summary>
        /// <param name="data">Plugin data object</param>
        public MyView(
            MyData data)
            : base(data)
        {
        }

        /// <summary>
        /// Initialize view controls
        /// </summary>
        /// <remarks>
        /// This method is called right before the view is
        /// to be rendered for the first time. It is guaranteed
        /// to be running on the primary UI thread, so you do not
        /// need to Invoke.
        ///
        /// You may also use the designer to layout your controls. If you
        /// move the call to InitializeComponent() to this method, you
        /// can improve plugin load-time, as the child controls will not
        /// get created until they are needed.
        /// </remarks>
        protected override void OnBuildControls()
        {
            this.treeviewCerts = new TreeView();
            this.treeviewCerts.ShowNodeToolTips = true;
            this.treeviewCerts.Dock = DockStyle.Fill;
            this.treeviewCerts.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeviewCerts_NodeMouseDoubleClick);
            this.treeviewCerts.NodeMouseClick += new TreeNodeMouseClickEventHandler(treeviewCerts_NodeMouseClick);
            this.treeviewCerts.KeyUp += new KeyEventHandler(treeviewCerts_KeyUp);
            this.Controls.Add(this.treeviewCerts);

            InitializeComponent();
        }

        void treeviewCerts_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                MyData data = (MyData)this.Data;
                string[] path = this.treeviewCerts.SelectedNode.FullPath.Split('\\');
                var cert = GetCertificateFromNode(treeviewCerts.SelectedNode);
                data.RemoveCert(path[0], path[1], cert);
            }
        }

        void treeviewCerts_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.Node.Level == 1)
            {
                treeviewCerts.SelectedNode = e.Node;
                contextMenu.Show(Cursor.Position);
            }
        }

        /// <summary>
        /// Fill in control data
        /// </summary>
        /// <param name="hint">Additional information for controls</param>
        /// <remarks>
        /// This method is called on the primary UI thread whenever the
        /// Remote Tool Framework needs to refresh the view to reflect
        /// data changes. The hint parameter is set to null if the
        /// Remote Tools Framework generated this method call. The
        /// data objects can also call their RenderViews method, which
        /// will cause this method to be called on all views that are
        /// hooked up to the data. RenderViews can set the hint parameter
        /// to whatever you like.
        /// </remarks>
        protected override void OnPopulateControls(object hint)
        {
            MyData data = (MyData)this.Data;

            this.treeviewCerts.Nodes.Clear();

            var locationNode = this.treeviewCerts.Nodes.Add("LocalMachine");
            foreach (var store in data.Stores.LocalMachine)
	        {
                var storeNode = locationNode.Nodes.Add(store.Name);
                foreach (var cert in store.Certificates)
                {
                    storeNode.Nodes.Add(cert.Subject);
                }
	        }

            locationNode = this.treeviewCerts.Nodes.Add("CurrentUser");
            foreach (var store in data.Stores.CurrentUser)
            {
                var storeNode = locationNode.Nodes.Add(store.Name);
                foreach (var cert in store.Certificates)
                {
                    TreeNode newNode = new TreeNode(cert.Subject);
                    newNode.Name = cert.Thumbprint;
                    newNode.ToolTipText = cert.Thumbprint;
                    storeNode.Nodes.Add(newNode);
                }
            }

            this.treeviewCerts.ExpandAll();
            this.treeviewCerts.TopNode = this.treeviewCerts.Nodes[0];
        }

        X509Certificate2 GetCertificateFromNode(TreeNode node)
        {
            MyData data = (MyData)this.Data;

            string[] path = node.FullPath.Split('\\');

            var location = data.Stores.CurrentUser;
            if (path[0] == "LocalMachine")
            {
                location = data.Stores.LocalMachine;
            }

            var certStore = location.Find(store => store.Name == path[1]);
            var cert = certStore.Certificates.Find(c => c.Subject == node.Text);

            return cert;
        }

        void treeviewCerts_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var cert = GetCertificateFromNode(e.Node);

            try
            {
                System.IO.File.WriteAllBytes(@"e:\tempCert.cer", cert.RawData);
                System.Diagnostics.Process.Start(@"e:\tempCert.cer");
            }
            catch (Exception) { }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addCertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addCertToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(120, 26);
            this.contextMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenu_ItemClicked);
            // 
            // addCertToolStripMenuItem
            // 
            this.addCertToolStripMenuItem.Name = "addCertToolStripMenuItem";
            this.addCertToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.addCertToolStripMenuItem.Text = "Add cert";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Certificates|*.cer";
            // 
            // MyView
            // 
            this.Name = "MyView";
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void contextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Add cert")
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    MyData data = (MyData)this.Data;
                    string[] path = this.treeviewCerts.SelectedNode.FullPath.Split('\\');
                    data.AddCert(path[0], path[1], openFileDialog.FileName);
                }
            }
        }
    }
}
