using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BuildWindowLauncher
{
    public partial class Form1 : Form
    {
        string comSpec = Environment.GetEnvironmentVariable("ComSpec");
        string wincedebug;
        string projFile;
        string _tgtPlat;
        string _tgtProc;

        public Form1()
        {
            InitializeComponent();
        }

        private void StartBuildWindow(string color, string root, bool powershell)
        {
            Properties.Settings.Default.Mode = gbMode.Controls.OfType<RadioButton>().Single(rb => rb.Checked).Text;
            Properties.Settings.Default.Project = gbProject.Controls.OfType<RadioButton>().Single(rb => rb.Checked).Text;
            Properties.Settings.Default.Platform = gbPlatform.Controls.OfType<RadioButton>().Single(rb => rb.Checked).Text;

            Properties.Settings.Default.LastEnlistmentRoot = root;
            Properties.Settings.Default.LastColor = color;
            Properties.Settings.Default.Save();

            string args = String.Format(@"/T:{0} /V:ON /K ""{1}\public\common\oak\misc\WMOpen.bat {1}\public\bld\{2} ""{3} {4} {5}"" {6}""", color, root, projFile, _tgtPlat, _tgtProc, wincedebug, powershell ? "&&wdcpsh" : "");
            Process p = Process.Start(comSpec, args);
            Application.Exit();
        }

        private void StartBuildWindow(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            StartBuildWindow(ColorConverter.ColorsToHex(button.BackColor, button.ForeColor), button.Text.Split(' ')[1], true);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Options frmOptions = new Options();
            frmOptions.ShowDialog();
            SetOptions();
        }

        static string[] defaultColors = { "02", "17", "04", "70" };
        private void SetOptions()
        {
            Console.WriteLine(Properties.Settings.Default.Mode);
            gbMode.Controls.OfType<RadioButton>().Single(rb => rb.Text == Properties.Settings.Default.Mode).Select();
            gbPlatform.Controls.OfType<RadioButton>().Single(rb => rb.Text == Properties.Settings.Default.Platform).Select();
            gbProject.Controls.OfType<RadioButton>().Single(rb => rb.Text == Properties.Settings.Default.Project).Select();

            int count = 0;
            flowEnlistments.Controls.Clear();
            foreach (string enlistment in Properties.Settings.Default.EnlistmentRoots)
            {
                Button button = new Button();
                if (count < defaultColors.Length)
                {
                    var colors = ColorConverter.HexToColors(defaultColors[count]);
                    button.BackColor = colors.BackColor;
                    button.ForeColor = colors.ForeColor;
                }
                else
                {
                    var colors = ColorConverter.HexToColors("07");
                    button.BackColor = colors.BackColor;
                    button.ForeColor = colors.ForeColor;
                }
                button.Text = String.Format("&{0} {1}", ++count, enlistment);
                button.FlatStyle = FlatStyle.Flat;
                button.AutoSize = true;
                button.TextAlign = ContentAlignment.MiddleLeft;
                button.Click += new EventHandler(StartBuildWindow);
                button.Font = new Font(button.Font, FontStyle.Bold);

                flowEnlistments.Controls.Add(button);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetOptions();

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.LastEnlistmentRoot) &&
                    !String.IsNullOrEmpty(Properties.Settings.Default.LastColor))
                {
                    StartBuildWindow(Properties.Settings.Default.LastColor, Properties.Settings.Default.LastEnlistmentRoot, true);
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void rbRelease_Click(object sender, EventArgs e)
        {
            wincedebug = "release";
        }

        private void rbChecked_Click(object sender, EventArgs e)
        {
            wincedebug = "checked";
        }

        private void rbDebug_Click(object sender, EventArgs e)
        {
            wincedebug = "debug";
        }

        private void rbProject_Click(object sender, EventArgs e)
        {
            projFile = "wm\\wm.pbxml";
        }

        private void rbProject2_Click(object sender, EventArgs e)
        {
            projFile = "uldr\\uldr.pbxml";
        }

        private void rbTsunagi_Click(object sender, EventArgs e)
        {
            _tgtPlat = "TSB Tsunagi";
            _tgtProc = "ARMV7";
        }

        private void rbCEPC_Click(object sender, EventArgs e)
        {
            _tgtPlat = "CEPC";
            _tgtProc = "x86";
        }

        private void rbPlatform2_Click(object sender, EventArgs e)
        {
            _tgtPlat = "ASUS E600";
            _tgtProc = "ARMV7";
        }

        private void rbPlatform4_CheckedChanged(object sender, EventArgs e)
        {
            _tgtPlat = "LGE Pacific";
            _tgtProc = "ARMV7";
        }
    }

    class Colors
    {
        public Color BackColor { get; set; }
        public Color ForeColor { get; set; }
    }

    static class ColorConverter
    {
        private static readonly Dictionary<char, Color> ColorMaps = new Dictionary<char, Color>()
        {
            { '0', Color.Black },
            { '1', Color.DarkBlue }, // Blue
            { '2', Color.Green },
            { '3', Color.Aqua },
            { '4', Color.DarkRed }, // Red
            { '5', Color.Purple },
            { '6', Color.Yellow },
            { '7', Color.LightGray }, // White
            { '8', Color.Gray },
            { '9', Color.LightBlue },
            { 'A', Color.LightGreen },
            { 'B', Color.LightCoral }, // Light Aqua
            { 'C', Color.Red },   // Light Red
            //{ 'D', Color.LightPurple }, // Light Purple
            { 'E', Color.LightYellow },
            { 'F', Color.White } // Bright White
        };

        public static Colors HexToColors(string hex)
        {
            return new Colors { BackColor = ColorMaps[hex[0]], ForeColor = ColorMaps[hex[1]] };
        }

        public static string ColorsToHex(Color backColor, Color foreColor)
        {
            string hex = "";
            hex += ColorMaps.Keys.Single(c => ColorMaps[c] == backColor);
            hex += ColorMaps.Keys.Single(c => ColorMaps[c] == foreColor);
            return hex;
        }
    }
}