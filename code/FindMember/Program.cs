using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.DirectoryServices;

namespace FindMember
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.WriteLine("USAGE: FindMember.exe <string: alias> <string: groupAlias> [bool: showAllMembers]");
                return;
            }
            string alias = args[0], groupAlias = args[1];
            if (groupAlias.StartsWith("redmond\\", StringComparison.InvariantCultureIgnoreCase))
            {
                groupAlias = groupAlias.Substring("redmond\\".Length);
            }
            bool showAllMembers = false;
            if (args.Length > 2)
            {
                bool.TryParse(args[2], out showAllMembers);
            }
            FindMember(groupAlias, alias, 0, showAllMembers);
        }

        static void FindMember(string group, string person, int tabStop, bool printAllMembers)
        {
            Console.CursorLeft = tabStop;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(group);
            Console.ForegroundColor = ConsoleColor.Gray;

            tabStop += 4;
            foreach (var alias in GetMembers(group).OrderBy(alias => alias))
            {
                Console.CursorLeft = tabStop;
                if (alias == person)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(alias);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                else if (printAllMembers)
                {
                    Console.WriteLine(alias);
                }
            }

            var subGroups = GetGroups(group);
            foreach (var subGroup in subGroups)
            //System.Threading.Tasks.Parallel.ForEach(subGroups, subGroup =>            // TODO: This works, but prints output out-of-order, need to write to a collection first
            {
                FindMember(subGroup, person, tabStop, printAllMembers);
            }
            //});
        }

        static IEnumerable<DirectoryEntry> GetDirectoryEntries(string group)
        {
            Regex rgxUser = new Regex(@"CN=[^,]+");
            Regex rgxDomain = new Regex(@"(?<=DC=)[^,]+");
            string[] properties = new string[] { "fullname" };
            DirectoryEntry adRootRedmond = new DirectoryEntry("LDAP://REDMOND");
            DirectorySearcher searcher = new DirectorySearcher(adRootRedmond);
            searcher.SearchScope = SearchScope.Subtree;
            searcher.ReferralChasing = ReferralChasingOption.All;
            searcher.PropertiesToLoad.AddRange(properties);
            searcher.Filter = String.Format("(SAMAccountName={0})", group);
             
            SearchResult result = searcher.FindOne();
            if (result != null)
            {
                DirectoryEntry directoryEntry = result.GetDirectoryEntry();

                var members = new System.Collections.ArrayList();
                if (directoryEntry.Properties["member"].Value is Array)
                {
                    members.AddRange(directoryEntry.Properties["member"].Value as Array);
                }
                else
                {
                    members.Add(directoryEntry.Properties["member"].Value);
                }

                foreach (string member in members)
                {
                    if (String.IsNullOrEmpty(member)) continue;
                    string search = rgxUser.Match(member).Value;
                    string domain = rgxDomain.Match(member).Value;
                    if (domain == "redmond")
                    {
                        if (!searcher.SearchRoot.Name.Contains("redmond"))
                        {
                            searcher.SearchRoot = adRootRedmond;
                        }
                    }
                    else
                    {
                        searcher.SearchRoot = new DirectoryEntry(String.Format("LDAP://{0}", domain.ToUpper()), null, null, AuthenticationTypes.Secure);
                    }
                    searcher.Filter = String.Format("({0})", search);
                    result = searcher.FindOne();
                    if (result == null) continue;
                    directoryEntry = result.GetDirectoryEntry();

                    yield return directoryEntry;
                }
            }
        }

        private static IEnumerable<string> GetMembers(string group)
        {
            var directoryEntries = GetDirectoryEntries(group);
            foreach (var directoryEntry in directoryEntries)
            {
                var mailNickname = directoryEntry.Properties["mailNickname"];
                if (mailNickname.Value != null)
                {
                    string alias = mailNickname.Value.ToString();
                    var classes = (object[])directoryEntry.Properties["ObjectClass"].Value;
                    if (!classes.Any(c => c.ToString() == "group"))
                    {
                        //string name = directoryEntry.Properties["cn"].Value.ToString();
                        yield return alias;
                    }
                }
            }
        }

        private static IEnumerable<string> GetGroups(string group)
        {
            var directoryEntries = GetDirectoryEntries(group);
            foreach (var directoryEntry in directoryEntries)
            {
                var mailNickname = directoryEntry.Properties["mailNickname"];
                if (mailNickname.Value != null)
                {
                    string alias = mailNickname.Value.ToString();
                    var classes = (object[])directoryEntry.Properties["ObjectClass"].Value;
                    if (classes.Any(c => c.ToString() == "group"))
                    {
                        yield return alias;
                    }
                }
            }
        }
    }
}
