using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace BuildWindowLauncher
{
    public partial class Options : Form
    {
        private void DeleteEnlistment(object sender, EventArgs e)
        {
            int buttonIndex = flowEnlistments.Controls.IndexOf((Control)sender);
            
            // Remove button
            flowEnlistments.Controls.RemoveAt(buttonIndex);

            // Remove textbox
            flowEnlistments.Controls.RemoveAt(buttonIndex);

            this.Height = flowEnlistments.Controls[buttonIndex - 2].Location.Y + 130;
        }

        private void AddEnlistment(string root)
        {
            Button button = new Button();
            button.Text = "Delete";
            button.Click += new EventHandler(DeleteEnlistment);
            flowEnlistments.Controls.Add(button);

            TextBox textBox = new TextBox();
            textBox.Width = flowEnlistments.Width - button.Width - 15;
            textBox.Text = root;
            flowEnlistments.Controls.Add(textBox);

            this.Height = button.Location.Y + 130;
        }

        public Options()
        {
            InitializeComponent();

            foreach (string enlistment in Properties.Settings.Default.EnlistmentRoots)
            {
                AddEnlistment(enlistment);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.EnlistmentRoots.Clear();

            foreach (var textBox in flowEnlistments.Controls.OfType<TextBox>())
            {
                Properties.Settings.Default.EnlistmentRoots.Add(textBox.Text);
            }

            Properties.Settings.Default.Save();

            this.Close();
        }

        private void Options_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddEnlistment(null);
        }
    }
}