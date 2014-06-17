using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.MobileDevices.AreaLibrary.Security.Accounts;

namespace AccountsToHTML
{
    class Program
    {
        static void Main(string[] args)
        {
            bool onlyGroups = 0 != args.Length;

            const string HEADING = "<p><h1><a name=\"{0}\">{0} : {1}</a></h1>";
            const string TABLE = "<h3>{0}</h3><table cellpadding=\"5\" border=\"1\">";
            const string ROW = "<tr><td><a href=\"#{0}\">{0}</a></td><td>{1}</td></tr>";
            const string ROW_NOT_GROUP = "<tr><td>{0}</td><td>{1}</td></tr>";

            string outPath = System.IO.Path.Combine(@"\release", onlyGroups ? "accountGroups.html" : "accounts.html");

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outPath))
            {
                sw.WriteLine("<html><head><title>" + (onlyGroups ? "AccountGroups" : "Accounts") + "</title></head><body>");
                foreach (Account account in AccountDatabase.GetAllAccounts())
                {
                    if (onlyGroups && !account.IsGroup) continue;
                    sw.WriteLine(HEADING, account.Name, account.FriendlyName.Trim('\0'));
                    var parents = new List<Account>(account.Parents);
                    if (parents.Count > 0)
                    {
                        sw.WriteLine(TABLE, "Parents");
                        foreach (Account parent in parents)
                        {
                            if (parent.IsGroup) sw.WriteLine(ROW, parent.Name, parent.FriendlyName.Trim('\0'));
                            else sw.WriteLine(ROW_NOT_GROUP, parent.Name, parent.FriendlyName.Trim('\0'));
                        }
                        sw.WriteLine("</table>");
                    }

                    var children = new List<Account>(account.Children);
                    if (children.Count > 0)
                    {
                        sw.WriteLine(TABLE, "Children");
                        foreach (Account child in children)
                        {
                            if (child.IsGroup) sw.WriteLine(ROW, child.Name, child.FriendlyName.Trim('\0'));
                            else sw.WriteLine(ROW_NOT_GROUP, child.Name, child.FriendlyName.Trim('\0'));
                        }
                        sw.WriteLine("</table>");
                    }

                    sw.WriteLine("</p><hr/>");
                }

                sw.WriteLine("</body></html>");
            }
        }
    }
}
