namespace BuildWindowLauncher
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.rbDebug = new System.Windows.Forms.RadioButton();
            this.rbRelease = new System.Windows.Forms.RadioButton();
            this.gbMode = new System.Windows.Forms.GroupBox();
            this.rbChecked = new System.Windows.Forms.RadioButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.gbProject = new System.Windows.Forms.GroupBox();
            this.rbProject2 = new System.Windows.Forms.RadioButton();
            this.rbProject = new System.Windows.Forms.RadioButton();
            this.gbPlatform = new System.Windows.Forms.GroupBox();
            this.rbPlatform4 = new System.Windows.Forms.RadioButton();
            this.rbTsunagi = new System.Windows.Forms.RadioButton();
            this.rbCEPC = new System.Windows.Forms.RadioButton();
            this.rbPlatform2 = new System.Windows.Forms.RadioButton();
            this.flowEnlistments = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbMode.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.gbProject.SuspendLayout();
            this.gbPlatform.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbDebug
            // 
            this.rbDebug.AutoSize = true;
            this.rbDebug.Location = new System.Drawing.Point(9, 65);
            this.rbDebug.Name = "rbDebug";
            this.rbDebug.Size = new System.Drawing.Size(57, 17);
            this.rbDebug.TabIndex = 9;
            this.rbDebug.Text = "&Debug";
            this.rbDebug.UseVisualStyleBackColor = true;
            this.rbDebug.Click += new System.EventHandler(this.rbDebug_Click);
            // 
            // rbRelease
            // 
            this.rbRelease.AutoSize = true;
            this.rbRelease.Checked = true;
            this.rbRelease.Location = new System.Drawing.Point(9, 19);
            this.rbRelease.Name = "rbRelease";
            this.rbRelease.Size = new System.Drawing.Size(64, 17);
            this.rbRelease.TabIndex = 10;
            this.rbRelease.TabStop = true;
            this.rbRelease.Text = "&Release";
            this.rbRelease.UseVisualStyleBackColor = true;
            this.rbRelease.Click += new System.EventHandler(this.rbRelease_Click);
            // 
            // gbMode
            // 
            this.gbMode.BackColor = System.Drawing.SystemColors.Control;
            this.gbMode.Controls.Add(this.rbChecked);
            this.gbMode.Controls.Add(this.rbDebug);
            this.gbMode.Controls.Add(this.rbRelease);
            this.gbMode.Location = new System.Drawing.Point(12, 36);
            this.gbMode.Name = "gbMode";
            this.gbMode.Size = new System.Drawing.Size(98, 94);
            this.gbMode.TabIndex = 7;
            this.gbMode.TabStop = false;
            this.gbMode.Text = "Mode";
            // 
            // rbChecked
            // 
            this.rbChecked.AutoSize = true;
            this.rbChecked.Location = new System.Drawing.Point(9, 42);
            this.rbChecked.Name = "rbChecked";
            this.rbChecked.Size = new System.Drawing.Size(68, 17);
            this.rbChecked.TabIndex = 11;
            this.rbChecked.Text = "&Checked";
            this.rbChecked.UseVisualStyleBackColor = true;
            this.rbChecked.Click += new System.EventHandler(this.rbChecked_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(277, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "&Options";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.optionsToolStripMenuItem_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::BuildWindowLauncher.Properties.Resources.stained_glass;
            this.pictureBox1.Location = new System.Drawing.Point(8, 335);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(102, 146);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // gbProject
            // 
            this.gbProject.BackColor = System.Drawing.SystemColors.Control;
            this.gbProject.Controls.Add(this.rbProject2);
            this.gbProject.Controls.Add(this.rbProject);
            this.gbProject.Location = new System.Drawing.Point(12, 260);
            this.gbProject.Name = "gbProject";
            this.gbProject.Size = new System.Drawing.Size(98, 69);
            this.gbProject.TabIndex = 16;
            this.gbProject.TabStop = false;
            this.gbProject.Text = "Project";
            // 
            // rbProject2
            // 
            this.rbProject2.AutoSize = true;
            this.rbProject2.Location = new System.Drawing.Point(6, 42);
            this.rbProject2.Name = "rbProject2";
            this.rbProject2.Size = new System.Drawing.Size(55, 17);
            this.rbProject2.TabIndex = 11;
            this.rbProject2.Text = "&ULDR";
            this.rbProject2.UseVisualStyleBackColor = true;
            this.rbProject2.Click += new System.EventHandler(this.rbProject2_Click);
            // 
            // rbProject
            // 
            this.rbProject.AutoSize = true;
            this.rbProject.Checked = true;
            this.rbProject.Location = new System.Drawing.Point(6, 19);
            this.rbProject.Name = "rbProject";
            this.rbProject.Size = new System.Drawing.Size(67, 17);
            this.rbProject.TabIndex = 9;
            this.rbProject.TabStop = true;
            this.rbProject.Text = "&Smartfon";
            this.rbProject.UseVisualStyleBackColor = true;
            this.rbProject.Click += new System.EventHandler(this.rbProject_Click);
            // 
            // gbPlatform
            // 
            this.gbPlatform.BackColor = System.Drawing.SystemColors.Control;
            this.gbPlatform.Controls.Add(this.rbPlatform4);
            this.gbPlatform.Controls.Add(this.rbTsunagi);
            this.gbPlatform.Controls.Add(this.rbCEPC);
            this.gbPlatform.Controls.Add(this.rbPlatform2);
            this.gbPlatform.Location = new System.Drawing.Point(12, 136);
            this.gbPlatform.Name = "gbPlatform";
            this.gbPlatform.Size = new System.Drawing.Size(98, 118);
            this.gbPlatform.TabIndex = 17;
            this.gbPlatform.TabStop = false;
            this.gbPlatform.Text = "Platform";
            // 
            // rbPlatform4
            // 
            this.rbPlatform4.AutoSize = true;
            this.rbPlatform4.Location = new System.Drawing.Point(6, 88);
            this.rbPlatform4.Name = "rbPlatform4";
            this.rbPlatform4.Size = new System.Drawing.Size(57, 17);
            this.rbPlatform4.TabIndex = 12;
            this.rbPlatform4.Text = "&Pacific";
            this.rbPlatform4.UseVisualStyleBackColor = true;
            this.rbPlatform4.CheckedChanged += new System.EventHandler(this.rbPlatform4_CheckedChanged);
            // 
            // rbTsunagi
            // 
            this.rbTsunagi.AutoSize = true;
            this.rbTsunagi.Location = new System.Drawing.Point(6, 65);
            this.rbTsunagi.Name = "rbTsunagi";
            this.rbTsunagi.Size = new System.Drawing.Size(63, 17);
            this.rbTsunagi.TabIndex = 11;
            this.rbTsunagi.Text = "&Tsunagi";
            this.rbTsunagi.UseVisualStyleBackColor = true;
            this.rbTsunagi.Click += new System.EventHandler(this.rbTsunagi_Click);
            // 
            // rbCEPC
            // 
            this.rbCEPC.AutoSize = true;
            this.rbCEPC.Checked = true;
            this.rbCEPC.Location = new System.Drawing.Point(6, 19);
            this.rbCEPC.Name = "rbCEPC";
            this.rbCEPC.Size = new System.Drawing.Size(53, 17);
            this.rbCEPC.TabIndex = 9;
            this.rbCEPC.TabStop = true;
            this.rbCEPC.Text = "CE&PC";
            this.rbCEPC.UseVisualStyleBackColor = true;
            this.rbCEPC.Click += new System.EventHandler(this.rbCEPC_Click);
            // 
            // rbPlatform2
            // 
            this.rbPlatform2.AutoSize = true;
            this.rbPlatform2.Location = new System.Drawing.Point(6, 42);
            this.rbPlatform2.Name = "rbPlatform2";
            this.rbPlatform2.Size = new System.Drawing.Size(50, 17);
            this.rbPlatform2.TabIndex = 10;
            this.rbPlatform2.Text = "&E600";
            this.rbPlatform2.UseVisualStyleBackColor = true;
            this.rbPlatform2.Click += new System.EventHandler(this.rbPlatform2_Click);
            // 
            // flowEnlistments
            // 
            this.flowEnlistments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowEnlistments.Location = new System.Drawing.Point(3, 16);
            this.flowEnlistments.Name = "flowEnlistments";
            this.flowEnlistments.Size = new System.Drawing.Size(136, 404);
            this.flowEnlistments.TabIndex = 18;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.flowEnlistments);
            this.groupBox1.Location = new System.Drawing.Point(124, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(142, 423);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Enlistments";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(277, 488);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbPlatform);
            this.Controls.Add(this.gbProject);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.gbMode);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Build Window Launcher";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.gbMode.ResumeLayout(false);
            this.gbMode.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.gbProject.ResumeLayout(false);
            this.gbProject.PerformLayout();
            this.gbPlatform.ResumeLayout(false);
            this.gbPlatform.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbDebug;
        private System.Windows.Forms.RadioButton rbRelease;
        private System.Windows.Forms.GroupBox gbMode;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.GroupBox gbProject;
        private System.Windows.Forms.RadioButton rbProject;
        private System.Windows.Forms.RadioButton rbProject2;
        private System.Windows.Forms.RadioButton rbChecked;
        private System.Windows.Forms.GroupBox gbPlatform;
        private System.Windows.Forms.RadioButton rbTsunagi;
        private System.Windows.Forms.RadioButton rbCEPC;
        private System.Windows.Forms.RadioButton rbPlatform2;
        private System.Windows.Forms.FlowLayoutPanel flowEnlistments;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbPlatform4;
    }
}

