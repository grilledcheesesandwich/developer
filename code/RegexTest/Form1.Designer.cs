namespace RegexTest
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtRegex = new System.Windows.Forms.TextBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.cbReplace = new System.Windows.Forms.CheckBox();
            this.cbIgnoreCase = new System.Windows.Forms.CheckBox();
            this.cbSingleLine = new System.Windows.Forms.CheckBox();
            this.cbMultiLine = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rtTextToMatch = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rtMatches = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.cbMatchesOnly = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Regular Expression";
            // 
            // txtRegex
            // 
            this.txtRegex.Location = new System.Drawing.Point(116, 6);
            this.txtRegex.Name = "txtRegex";
            this.txtRegex.Size = new System.Drawing.Size(186, 20);
            this.txtRegex.TabIndex = 1;
            this.txtRegex.TextChanged += new System.EventHandler(this.txtRegex_TextChanged);
            // 
            // txtReplace
            // 
            this.txtReplace.Location = new System.Drawing.Point(308, 6);
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(155, 20);
            this.txtReplace.TabIndex = 2;
            // 
            // cbReplace
            // 
            this.cbReplace.AutoSize = true;
            this.cbReplace.Location = new System.Drawing.Point(469, 8);
            this.cbReplace.Name = "cbReplace";
            this.cbReplace.Size = new System.Drawing.Size(66, 17);
            this.cbReplace.TabIndex = 3;
            this.cbReplace.Text = "Replace";
            this.cbReplace.UseVisualStyleBackColor = true;
            this.cbReplace.CheckedChanged += new System.EventHandler(this.checkBoxCheckedChanged);
            // 
            // cbIgnoreCase
            // 
            this.cbIgnoreCase.AutoSize = true;
            this.cbIgnoreCase.Checked = true;
            this.cbIgnoreCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIgnoreCase.Location = new System.Drawing.Point(647, 8);
            this.cbIgnoreCase.Name = "cbIgnoreCase";
            this.cbIgnoreCase.Size = new System.Drawing.Size(83, 17);
            this.cbIgnoreCase.TabIndex = 4;
            this.cbIgnoreCase.Text = "Ignore Case";
            this.cbIgnoreCase.UseVisualStyleBackColor = true;
            this.cbIgnoreCase.CheckedChanged += new System.EventHandler(this.checkBoxCheckedChanged);
            // 
            // cbSingleLine
            // 
            this.cbSingleLine.AutoSize = true;
            this.cbSingleLine.Location = new System.Drawing.Point(733, 8);
            this.cbSingleLine.Name = "cbSingleLine";
            this.cbSingleLine.Size = new System.Drawing.Size(74, 17);
            this.cbSingleLine.TabIndex = 5;
            this.cbSingleLine.Text = "Single-line";
            this.cbSingleLine.UseVisualStyleBackColor = true;
            this.cbSingleLine.CheckedChanged += new System.EventHandler(this.checkBoxCheckedChanged);
            // 
            // cbMultiLine
            // 
            this.cbMultiLine.AutoSize = true;
            this.cbMultiLine.Location = new System.Drawing.Point(819, 8);
            this.cbMultiLine.Name = "cbMultiLine";
            this.cbMultiLine.Size = new System.Drawing.Size(67, 17);
            this.cbMultiLine.TabIndex = 6;
            this.cbMultiLine.Text = "Multi-line";
            this.cbMultiLine.UseVisualStyleBackColor = true;
            this.cbMultiLine.CheckedChanged += new System.EventHandler(this.checkBoxCheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rtTextToMatch);
            this.groupBox1.Location = new System.Drawing.Point(15, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(550, 600);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Text to match";
            // 
            // rtTextToMatch
            // 
            this.rtTextToMatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtTextToMatch.Location = new System.Drawing.Point(3, 16);
            this.rtTextToMatch.Name = "rtTextToMatch";
            this.rtTextToMatch.Size = new System.Drawing.Size(544, 581);
            this.rtTextToMatch.TabIndex = 0;
            this.rtTextToMatch.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.rtMatches);
            this.groupBox2.Location = new System.Drawing.Point(571, 32);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(550, 600);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Matches";
            // 
            // rtMatches
            // 
            this.rtMatches.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtMatches.Location = new System.Drawing.Point(3, 16);
            this.rtMatches.Name = "rtMatches";
            this.rtMatches.Size = new System.Drawing.Size(544, 581);
            this.rtMatches.TabIndex = 0;
            this.rtMatches.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 642);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1131, 22);
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslblStatus
            // 
            this.tsslblStatus.Name = "tsslblStatus";
            this.tsslblStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // cbMatchesOnly
            // 
            this.cbMatchesOnly.AutoSize = true;
            this.cbMatchesOnly.Location = new System.Drawing.Point(541, 8);
            this.cbMatchesOnly.Name = "cbMatchesOnly";
            this.cbMatchesOnly.Size = new System.Drawing.Size(89, 17);
            this.cbMatchesOnly.TabIndex = 10;
            this.cbMatchesOnly.Text = "Matches only";
            this.cbMatchesOnly.UseVisualStyleBackColor = true;
            this.cbMatchesOnly.CheckedChanged += new System.EventHandler(this.checkBoxCheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1131, 664);
            this.Controls.Add(this.cbMatchesOnly);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbMultiLine);
            this.Controls.Add(this.cbSingleLine);
            this.Controls.Add(this.cbIgnoreCase);
            this.Controls.Add(this.cbReplace);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.txtRegex);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "RegExTest";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRegex;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.CheckBox cbReplace;
        private System.Windows.Forms.CheckBox cbIgnoreCase;
        private System.Windows.Forms.CheckBox cbSingleLine;
        private System.Windows.Forms.CheckBox cbMultiLine;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RichTextBox rtMatches;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslblStatus;
        private System.Windows.Forms.RichTextBox rtTextToMatch;
        private System.Windows.Forms.CheckBox cbMatchesOnly;
    }
}

