//-----------------------------------------------------------------------------
// <copyright file='MyView.cs' company='Microsoft'>
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
// Use of this sample source code is subject to the terms of the Microsoft
// license agreement under which you licensed this sample source code. If
// you did not accept the terms of the license agreement, you are not
// authorized to use this sample source code. For the terms of the license,
// please see the license agreement between you and Microsoft or, if applicable,
// see the LICENSE.RTF on your install media or the root of your tools installation.
// THE SAMPLE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES.
// </copyright>
// <summary>
// Plugin view class
// </summary>
//-----------------------------------------------------------------------------

namespace PushXmlPlugin.View
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;

    using PushXmlPlugin.Data;
    using Microsoft.RemoteToolSdk.PluginComponents;

    using System.Xml;

    /// <summary>
    /// Plugin view class for My Plugin.
    /// </summary>
    public class MyView : PluginDataView
    {
        private Button btnSend;
        private RadioButton rdbCfgMgr1;
        private RadioButton rdbCfgMgr2;
        private RichTextBox txtOutXml;
        private RichTextBox txtInXml;
        private Label label1;
        private RichTextBox txtError1;
        private RichTextBox txtError2;
        private Button btnCert;
        private OpenFileDialog ofdCertificate;
        private RichTextBox txtError;
        private RadioButton rdbPriv;
        private RadioButton rdbUnPriv;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private Button btnForward;
        private Button btnBack;
        private Button btnDelete;
        private Label lblCurrent;
        private SplitContainer splitContainer1;
        private GroupBox groupBox3;
        private ComboBox cboRole;
        private OpenFileDialog ofdXml;
        private Button btnOpenXml;

        private static string previousText = "";
        private MyData data;
        private List<string> history = new List<string>();
        private Button btnSave;
        private SaveFileDialog sfdXml;

        private int current = -1;

        /// <summary>
        /// Construct a view. This object is created on the primary UI thread.
        /// </summary>
        /// <param name="data">Plugin data object</param>
        public MyView(MyData data)
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
            this.InitializeComponent();

            data = (MyData)this.Data;

            string path = Path.Combine(System.Environment.GetEnvironmentVariable("TEMP"), "history.xml");

            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(File.ReadAllText(path));
                foreach (XmlNode node in doc.SelectNodes(@"/History/Element"))
                {
                    history.Add(node.InnerText);
                }
            }

            cboRole.Items.Add(new RoleItem("MANAGER", 0x0008));
            cboRole.Items.Add(new RoleItem("USER_AUTH", 0x0010));

            cboRole.DisplayMember = "RoleName";
            cboRole.SelectedIndex = 0;

            current = history.Count - 1;
            UpdateCurrent();
            FormatTextBox();
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

        }

        /// <summary>
        /// Iterate through the controls that are in the form and try to get the
        /// selected text into the clipbaord. Called from the data object via
        /// the framework.
        /// </summary>
        public void CopyToClipBoard()
        {
            foreach (Control c in Controls)
            {
                if ((c.Focused == true) && (c.CanSelect))
                {
                    if (c is TextBox)
                    {
                        Clipboard.SetText(((TextBox)c).SelectedText);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize view controls
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSend = new System.Windows.Forms.Button();
            this.rdbCfgMgr1 = new System.Windows.Forms.RadioButton();
            this.rdbCfgMgr2 = new System.Windows.Forms.RadioButton();
            this.txtError = new System.Windows.Forms.RichTextBox();
            this.txtOutXml = new System.Windows.Forms.RichTextBox();
            this.txtInXml = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtError1 = new System.Windows.Forms.RichTextBox();
            this.txtError2 = new System.Windows.Forms.RichTextBox();
            this.btnCert = new System.Windows.Forms.Button();
            this.ofdCertificate = new System.Windows.Forms.OpenFileDialog();
            this.rdbPriv = new System.Windows.Forms.RadioButton();
            this.rdbUnPriv = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cboRole = new System.Windows.Forms.ComboBox();
            this.ofdXml = new System.Windows.Forms.OpenFileDialog();
            this.btnOpenXml = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.sfdXml = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSend.Enabled = false;
            this.btnSend.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSend.ForeColor = System.Drawing.Color.Red;
            this.btnSend.Location = new System.Drawing.Point(145, 581);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // rdbCfgMgr1
            // 
            this.rdbCfgMgr1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdbCfgMgr1.AutoSize = true;
            this.rdbCfgMgr1.Checked = true;
            this.rdbCfgMgr1.Location = new System.Drawing.Point(3, 584);
            this.rdbCfgMgr1.Name = "rdbCfgMgr1";
            this.rdbCfgMgr1.Size = new System.Drawing.Size(65, 17);
            this.rdbCfgMgr1.TabIndex = 5;
            this.rdbCfgMgr1.TabStop = true;
            this.rdbCfgMgr1.Text = "CfgMgr1";
            this.rdbCfgMgr1.UseVisualStyleBackColor = true;
            // 
            // rdbCfgMgr2
            // 
            this.rdbCfgMgr2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdbCfgMgr2.AutoSize = true;
            this.rdbCfgMgr2.Location = new System.Drawing.Point(74, 584);
            this.rdbCfgMgr2.Name = "rdbCfgMgr2";
            this.rdbCfgMgr2.Size = new System.Drawing.Size(65, 17);
            this.rdbCfgMgr2.TabIndex = 6;
            this.rdbCfgMgr2.Text = "CfgMgr2";
            this.rdbCfgMgr2.UseVisualStyleBackColor = true;
            // 
            // txtError
            // 
            this.txtError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtError.Location = new System.Drawing.Point(292, 583);
            this.txtError.Multiline = false;
            this.txtError.Name = "txtError";
            this.txtError.ReadOnly = true;
            this.txtError.Size = new System.Drawing.Size(67, 20);
            this.txtError.TabIndex = 8;
            this.txtError.Text = "";
            // 
            // txtOutXml
            // 
            this.txtOutXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutXml.Location = new System.Drawing.Point(0, 0);
            this.txtOutXml.Name = "txtOutXml";
            this.txtOutXml.ReadOnly = true;
            this.txtOutXml.Size = new System.Drawing.Size(365, 570);
            this.txtOutXml.TabIndex = 9;
            this.txtOutXml.Text = "";
            this.txtOutXml.WordWrap = false;
            this.txtOutXml.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtOutXml_MouseUp);
            // 
            // txtInXml
            // 
            this.txtInXml.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInXml.Location = new System.Drawing.Point(0, 0);
            this.txtInXml.Name = "txtInXml";
            this.txtInXml.Size = new System.Drawing.Size(363, 570);
            this.txtInXml.TabIndex = 10;
            this.txtInXml.Text = "";
            this.txtInXml.WordWrap = false;
            this.txtInXml.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtInXml_MouseUp);
            this.txtInXml.TextChanged += new System.EventHandler(this.txtInXml_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(226, 586);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Error Code:";
            // 
            // txtError1
            // 
            this.txtError1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtError1.Location = new System.Drawing.Point(365, 583);
            this.txtError1.Multiline = false;
            this.txtError1.Name = "txtError1";
            this.txtError1.ReadOnly = true;
            this.txtError1.Size = new System.Drawing.Size(206, 20);
            this.txtError1.TabIndex = 12;
            this.txtError1.Text = "";
            // 
            // txtError2
            // 
            this.txtError2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtError2.Location = new System.Drawing.Point(577, 583);
            this.txtError2.Multiline = false;
            this.txtError2.Name = "txtError2";
            this.txtError2.ReadOnly = true;
            this.txtError2.Size = new System.Drawing.Size(144, 20);
            this.txtError2.TabIndex = 13;
            this.txtError2.Text = "";
            // 
            // btnCert
            // 
            this.btnCert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCert.Location = new System.Drawing.Point(4, 636);
            this.btnCert.Name = "btnCert";
            this.btnCert.Size = new System.Drawing.Size(99, 23);
            this.btnCert.TabIndex = 14;
            this.btnCert.Text = "Get Cert Xml";
            this.btnCert.UseVisualStyleBackColor = true;
            this.btnCert.Click += new System.EventHandler(this.btnCert_Click);
            // 
            // ofdCertificate
            // 
            this.ofdCertificate.DefaultExt = "cer";
            this.ofdCertificate.Filter = "Certificate Files (*.cer) |*.cer";
            // 
            // rdbPriv
            // 
            this.rdbPriv.AutoSize = true;
            this.rdbPriv.Checked = true;
            this.rdbPriv.Location = new System.Drawing.Point(6, 19);
            this.rdbPriv.Name = "rdbPriv";
            this.rdbPriv.Size = new System.Drawing.Size(71, 17);
            this.rdbPriv.TabIndex = 15;
            this.rdbPriv.TabStop = true;
            this.rdbPriv.Text = "Privileged";
            this.rdbPriv.UseVisualStyleBackColor = true;
            // 
            // rdbUnPriv
            // 
            this.rdbUnPriv.AutoSize = true;
            this.rdbUnPriv.Location = new System.Drawing.Point(83, 19);
            this.rdbUnPriv.Name = "rdbUnPriv";
            this.rdbUnPriv.Size = new System.Drawing.Size(84, 17);
            this.rdbUnPriv.TabIndex = 16;
            this.rdbUnPriv.Text = "Unprivileged";
            this.rdbUnPriv.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.rdbPriv);
            this.groupBox1.Controls.Add(this.rdbUnPriv);
            this.groupBox1.Location = new System.Drawing.Point(109, 616);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(177, 42);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Store to Install Cert Into";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.lblCurrent);
            this.groupBox2.Controls.Add(this.btnDelete);
            this.groupBox2.Controls.Add(this.btnForward);
            this.groupBox2.Controls.Add(this.btnBack);
            this.groupBox2.Location = new System.Drawing.Point(412, 616);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(308, 42);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "History";
            // 
            // lblCurrent
            // 
            this.lblCurrent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCurrent.Location = new System.Drawing.Point(87, 16);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Size = new System.Drawing.Size(52, 17);
            this.lblCurrent.TabIndex = 6;
            this.lblCurrent.Text = "0 / 0";
            this.lblCurrent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(226, 13);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnForward
            // 
            this.btnForward.Enabled = false;
            this.btnForward.Location = new System.Drawing.Point(145, 13);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(75, 23);
            this.btnForward.TabIndex = 4;
            this.btnForward.Text = ">>";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnBack
            // 
            this.btnBack.Enabled = false;
            this.btnBack.Location = new System.Drawing.Point(6, 13);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(75, 23);
            this.btnBack.TabIndex = 3;
            this.btnBack.Text = "<<";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtInXml);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtOutXml);
            this.splitContainer1.Size = new System.Drawing.Size(732, 570);
            this.splitContainer1.SplitterDistance = 363;
            this.splitContainer1.TabIndex = 19;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.cboRole);
            this.groupBox3.Location = new System.Drawing.Point(292, 616);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(114, 42);
            this.groupBox3.TabIndex = 19;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Role";
            // 
            // cboRole
            // 
            this.cboRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRole.FormattingEnabled = true;
            this.cboRole.Location = new System.Drawing.Point(3, 16);
            this.cboRole.Name = "cboRole";
            this.cboRole.Size = new System.Drawing.Size(105, 21);
            this.cboRole.TabIndex = 0;
            // 
            // ofdXml
            // 
            this.ofdXml.DefaultExt = "xml";
            this.ofdXml.Filter = "Xml Files (*.xml) |*.xml";
            // 
            // btnOpenXml
            // 
            this.btnOpenXml.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenXml.Location = new System.Drawing.Point(4, 607);
            this.btnOpenXml.Name = "btnOpenXml";
            this.btnOpenXml.Size = new System.Drawing.Size(47, 23);
            this.btnOpenXml.TabIndex = 20;
            this.btnOpenXml.Text = "Open Xml File";
            this.btnOpenXml.UseVisualStyleBackColor = true;
            this.btnOpenXml.Click += new System.EventHandler(this.btnOpenXml_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(56, 607);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(47, 23);
            this.btnSave.TabIndex = 21;
            this.btnSave.Text = "Save Xml File";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // sfdXml
            // 
            this.sfdXml.DefaultExt = "xml";
            this.sfdXml.FileName = "test.xml";
            this.sfdXml.Filter = "Xml Files (*.xml) |*.xml";
            // 
            // MyView
            // 
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnOpenXml);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCert);
            this.Controls.Add(this.txtError2);
            this.Controls.Add(this.txtError1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.rdbCfgMgr2);
            this.Controls.Add(this.rdbCfgMgr1);
            this.Controls.Add(this.btnSend);
            this.Name = "MyView";
            this.Size = new System.Drawing.Size(732, 670);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        // When the user clicks the button, we want to take the string from
        // the edit control and push it to the device.
        // I implemented a method on the data object that does this,
        // since the data object should communicate with the device, and
        // not the view (code snobbery).

        /// <summary>
        /// Send to device button clicked
        /// </summary>
        /// <param name="sender">Origin of event</param>
        /// <param name="e">Event arguments</param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            txtOutXml.Clear();
            txtError.Clear();
            txtError1.Clear();
            txtError2.Clear();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(txtInXml.Text);
            }
            catch (Exception ex)
            {
                txtOutXml.Text = "Exception caught: " + ex.Message;
                return;
            }

            CommandInfo result = data.SendXmlToDevice(txtInXml.Text, rdbCfgMgr1.Checked, ((RoleItem)cboRole.SelectedItem).RoleValue);

            System.ComponentModel.Win32Exception error = new System.ComponentModel.Win32Exception((int)result.ErrorCode);

            txtError.Text = result.ErrorCode.ToString("X");
            txtError1.Text = error.Message;
            txtError2.Text = ((ErrorCodes)result.ErrorCode).ToString();

            if (!ContainsIgnoreReturns(history, txtInXml.Text))
            {
                history.Add(txtInXml.Text);
                current = history.Count - 1;
                WriteHistory();
                UpdateCurrent();
            }

            if (null != result.XmlDoc)
            {
                txtOutXml.Text = PrettyPrintXml(result.XmlDoc);
                HighlightErrors();
            }
            else
            {
                // There was an error
                txtOutXml.Text = result.ErrorMessage;
            }

            this.Cursor = Cursors.Default;
        }

        private void WriteHistory()
        {
            string path = Path.Combine(System.Environment.GetEnvironmentVariable("TEMP"), "history.xml");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;

            using (XmlWriter writer = XmlWriter.Create(path, settings))
            {
                writer.WriteStartElement("History");

                foreach (string s in history)
                {
                    writer.WriteElementString("Element", s);
                }

                writer.WriteEndElement();
            }
        }

        private void UpdateCurrent()
        {
            lblCurrent.Text = String.Format("{0} / {1}", current + 1, history.Count);

            btnBack.Enabled = btnForward.Enabled = (history.Count > 1);

            btnDelete.Enabled = (history.Count > 0);

            if (history.Count > 0)
            {
                txtInXml.Text = history[current];
            }
        }

        #region Pretty_Print_Xml
        /// <summary>
        /// Helper method to convert XML to a string including indenting
        /// </summary>
        /// <param name="doc">The XmlDocument to convert</param>
        /// <returns>Indented and formatted XML string</returns>
        public string PrettyPrintXml(XmlDocument doc)
        {
            if (doc.DocumentElement.Name == "wap-provisioningdoc")
                rdbCfgMgr1.Checked = true;
            else if (doc.DocumentElement.Name == "SyncML")
                rdbCfgMgr2.Checked = true;

            // set up a stream that writes to a string
            StringBuilder outputBuf = new StringBuilder();
            StringWriter sw = new StringWriter(outputBuf);

            // create a XmlWriter and set it to indent the Xml
            XmlTextWriter tw = new XmlTextWriter(sw);
            tw.Formatting = Formatting.Indented;

            // shove the Xml through the formatter stream to the string
            doc.WriteContentTo(tw);

            // close/flush our objects
            tw.Close();
            sw.Close();

            // we end up with a nicely formatted string
            return outputBuf.ToString();
        }

        /// <summary>
        /// Helper method to convert XML to a string including indenting
        /// </summary>
        /// <param name="inputXml">The string of XML</param>
        /// <returns>Indented and formatted XML string</returns>
        public string PrettyPrintXml(string inputXml)
        {
            XmlDocument doc = new XmlDocument();

            doc.LoadXml(inputXml);

            return PrettyPrintXml(doc);
        }
        #endregion //Pretty_Print_Xml

        private void FormatTextBox()
        {
            try
            {
                string xml = PrettyPrintXml(txtInXml.Text);
                btnSend.ForeColor = Color.Green;
                txtOutXml.Clear();

                if (previousText.Length == 0)
                {
                    txtInXml.Text = xml;
                }
            }
            catch (Exception ex)
            {
                txtOutXml.Text = ex.Message;

                btnSend.ForeColor = Color.Red;
            }

            btnSend.Enabled = ("" != txtInXml.Text);

            previousText = txtInXml.Text;
        }

        private void txtInXml_TextChanged(object sender, EventArgs e)
        {
            FormatTextBox();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (current-- <= 0) current = history.Count - 1;

            UpdateCurrent();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (++current >= history.Count) current = 0;

            UpdateCurrent();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int count = history.Count;
            history.Remove(txtInXml.Text);

            if (count == history.Count)
            {
                history.RemoveAt(current);
            }

            if (current-- <= 0) current = history.Count - 1;

            WriteHistory();

            txtInXml.Clear();

            UpdateCurrent();
        }

        private void btnCert_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == ofdCertificate.ShowDialog())
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(ofdCertificate.FileName);

                string store = (rdbPriv.Checked) ? "Privileged Execution Trust Authorities" : "Unprivileged Execution Trust Authorities";

                txtInXml.Text = String.Format(@"<wap-provisioningdoc>
                                                    <characteristic type=""CertificateStore"">
                                                        <characteristic type=""{0}"">
                                                            <characteristic type=""{1}"">
                                                                <parm name=""EncodedCertificate"" value=""{2}""/>
                                                            </characteristic>
                                                        </characteristic>
                                                    </characteristic>
                                                </wap-provisioningdoc>", store, cert.Thumbprint, Convert.ToBase64String(cert.RawData));
            }
        }

        private void btnOpenXml_Click(object sender, EventArgs e)
        {
            ofdXml.InitialDirectory = System.Environment.GetEnvironmentVariable("_FLATRELEASEDIR");

            if (DialogResult.OK == ofdXml.ShowDialog())
            {
                using (StreamReader sr = new StreamReader(ofdXml.FileName))
                {
                    txtInXml.Text = sr.ReadToEnd();
                }
            }
        }

        private bool ContainsIgnoreReturns(List<string> list, string s)
        {
            foreach (string item in list)
            {
                if (item.Replace("\r\n", "\n") == s.Replace("\r\n", "\n"))
                    return true;
            }

            return false;
        }

        private void txtInXml_MouseUp(object sender, MouseEventArgs e)
        {
            if ((MouseButtons.Right == e.Button) && ("" == txtInXml.SelectedText))
                txtInXml.Paste();
        }

        private void txtOutXml_MouseUp(object sender, MouseEventArgs e)
        {
            if ((txtOutXml.SelectedText != "") && (e.Button == MouseButtons.Left))
            {
                txtOutXml.Copy();
                txtOutXml.DeselectAll();
            }
        }

        private void HighlightErrors()
        {
            int start = txtOutXml.Find("error");
            while (-1 != start)
            {
                txtOutXml.Select(start, 5);
                txtOutXml.SelectionBackColor = Color.Red;

                start = txtOutXml.Text.IndexOf("error", start + 1);
            }

            start = txtOutXml.Text.IndexOf("<Data>") + 6;
            while (5 != start)  // -1 + 6
            {
                int end = txtOutXml.Text.IndexOf("</Data>", start);
                if (txtOutXml.Text.Substring(start, end - start) != "200")
                {
                    txtOutXml.Select(start, end - start);
                    txtOutXml.SelectionBackColor = Color.Red;
                }

                start = txtOutXml.Text.IndexOf("<Data>", end) + 6;
            }

            txtOutXml.DeselectAll();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            sfdXml.InitialDirectory = System.Environment.GetEnvironmentVariable("_FLATRELEASEDIR");

            if (DialogResult.OK == sfdXml.ShowDialog())
            {
                using (StreamWriter sw = new StreamWriter(sfdXml.FileName))
                {
                    sw.Write(txtInXml.Text);
                }
            }
        }
    }

    struct RoleItem
    {
        private string roleName;

        public string RoleName
        {
            get { return roleName; }
            set { roleName = value; }
        }
        private uint roleValue;

        public uint RoleValue
        {
            get { return roleValue; }
            set { roleValue = value; }
        }

        public RoleItem(string n, uint v)
        {
            roleName = n;
            roleValue = v;
        }
    }
}