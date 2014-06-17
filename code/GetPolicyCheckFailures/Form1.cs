using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace GetPolicyCheckFailures
{
    static class Extensions
    {
        public static string MatchAndReplace(this Regex rgx, ref string s)
        {
            var match = rgx.Match(s).Groups[1];
            s = s.Substring(match.Index + match.Length);
            return match.Value;
        }
    }

    public partial class Form1 : Form
    {
        Regex rgxAccess = new Regex(@"Access=([\w]*)", RegexOptions.Compiled);
        Regex rgxIRI = new Regex(@"Rsrc=""([^""\n]+)", RegexOptions.Compiled);
        Regex rgxAccount = new Regex(@"Acct\(s\)=([-\w]*)", RegexOptions.Compiled);

        public Form1()
        {
            InitializeComponent();
            rtbInOut.WordWrap = false;
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            rtbInOut.Clear();
            var fails = new List<PolicyCheck>();

            string raw = Clipboard.GetText();
            string[] lines = raw.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );
            if (lines.Length == 0) return;
            string lineLeft = lines[0];
            for (int i = 0; i < lines.Length; )
            {
                if (lineLeft.Contains("PolicyEngine!PolicyCheck"))
                {
                    var fail = new PolicyCheck();
                    fail.IsError = lineLeft.Contains("Error");
                    fail.Access = rgxAccess.MatchAndReplace(ref lineLeft);
                    while (!lineLeft.Contains("Rsrc="))
                    {
                        lineLeft = lines[++i];
                    }
                    fail.IRI = rgxIRI.MatchAndReplace(ref lineLeft);
                    while (!lineLeft.Contains("Acct(s)="))
                    {
                        lineLeft = lines[++i];
                    }
                    fail.Account = rgxAccount.MatchAndReplace(ref lineLeft);
                    fails.Add(fail);
                }
                else
                {
                    if (++i < lines.Length) lineLeft = lines[i];
                }
            }

            fails.Sort();
            var distinctFails = fails.Distinct();
            foreach (var fail in distinctFails)
            {
                int start = rtbInOut.TextLength;
                rtbInOut.AppendText(fail.IRI);
                rtbInOut.Select(start, fail.IRI.Length);
                rtbInOut.SelectionColor = fail.IsError ? Color.Red : Color.Black;

                rtbInOut.AppendText(" ");

                start = rtbInOut.TextLength;
                rtbInOut.AppendText(fail.Account);
                rtbInOut.Select(start, fail.Account.Length);
                rtbInOut.SelectionColor = Color.Green;

                rtbInOut.AppendText(" ");

                start = rtbInOut.TextLength;
                rtbInOut.AppendText(fail.Access);
                rtbInOut.Select(start, fail.Access.Length);
                rtbInOut.SelectionColor = Color.Blue;

                rtbInOut.AppendText(Environment.NewLine);
            }

            btnCopy.Visible = true;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbInOut.Text, TextDataFormat.Rtf);
        }
    }

    class PolicyCheck :  IEquatable<PolicyCheck>, IEqualityComparer<PolicyCheck>, IComparable<PolicyCheck>, IComparable, IComparer<PolicyCheck>
    {
        public string Access { get; set; }
        public string IRI { get; set; }
        public string Account { get; set; }
        public bool IsError { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Access.GetHashCode() ^ IRI.GetHashCode() ^ Account.GetHashCode();
        }

        public bool Equals(PolicyCheck x, PolicyCheck y)
        {
            return x.Access == y.Access && x.IRI == y.IRI && x.Account == y.Account;
        }

        public int GetHashCode(PolicyCheck obj)
        {
            return obj.Access.GetHashCode() ^ obj.IRI.GetHashCode() ^ obj.Account.GetHashCode();
        }

        #region IEquatable<PolicyCheck> Members

        public bool Equals(PolicyCheck other)
        {
            return Access == other.Access && IRI == other.IRI && Account == other.Account;
        }

        #endregion

        #region IComparable<PolicyCheck> Members

        public int CompareTo(PolicyCheck other)
        {
            if (this.IRI != other.IRI) return this.IRI.CompareTo(other.IRI);
            else if (this.Account != other.Account) return this.Account.CompareTo(other.Account);
            else return this.Access.CompareTo(other.Access);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IComparer<PolicyCheck> Members

        public int Compare(PolicyCheck x, PolicyCheck y)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
