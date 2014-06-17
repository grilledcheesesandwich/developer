using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace RegexTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void UpdateMatches()
        {
            this.rtMatches.Clear();
            this.tsslblStatus.Text = "";
            if ((this.txtRegex.Text.Length != 0) && (this.rtTextToMatch.Text.Length != 0))
            {
                RegexOptions regexOptions = RegexOptions.Compiled;
                if (this.cbIgnoreCase.Checked)
                {
                    regexOptions |= RegexOptions.IgnoreCase;
                }
                if (this.cbSingleLine.Checked)
                {
                    regexOptions |= RegexOptions.Singleline;
                }
                if (this.cbMultiLine.Checked)
                {
                    regexOptions |= RegexOptions.Multiline;
                }

                Regex rgxTest;                
                try
                {
                    rgxTest = new Regex(this.txtRegex.Text, regexOptions);
                    this.txtRegex.BackColor = Color.White;
                }
                catch (Exception)
                {
                    this.txtRegex.BackColor = Color.DarkSalmon;
                    return;
                }
                if (this.cbReplace.Checked)
                {
                    this.rtMatches.Text = rgxTest.Replace(this.rtTextToMatch.Text, this.txtReplace.Text);
                }
                else
                {
                    MatchCollection matches = rgxTest.Matches(this.rtTextToMatch.Text);
                    
                    if (matches.Count > 0)
                    {
                        this.tsslblStatus.Text = string.Format("Found {0} matches, {1} groups", matches.Count, matches[0].Groups.Count - 1);

                        if (cbMatchesOnly.Checked)
                        {
                            foreach (Match match in matches)
                            {
                                this.rtMatches.AppendText(match.Value + Environment.NewLine);
                            }
                        }
                        else
                        {
                            this.rtMatches.Text = this.rtTextToMatch.Text;
                            bool flag = true;
                            foreach (Match match in matches)
                            {
                                this.rtMatches.Select(match.Index, match.Length);
                                this.rtMatches.SelectionBackColor = flag ? Color.Red : Color.Yellow;
                                this.rtMatches.SelectionColor = !flag ? Color.Red : Color.Yellow;
                                this.rtMatches.DeselectAll();
                                flag = !flag;
                            }
                        }
                    }
                }
            }
        }

        private void txtRegex_TextChanged(object sender, EventArgs e)
        {
            UpdateMatches();
        }

        private void checkBoxCheckedChanged(object sender, EventArgs e)
        {
            UpdateMatches();
        }
    }
}
