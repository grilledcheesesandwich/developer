using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Linq;

namespace CodeSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 6)
            {
                Console.WriteLine("\nUsage: CodeSearch.exe <search file> <ignore file> <server> <database> <table> <file-spec>\n");
                Console.WriteLine("Run in root of tree you want to search\n");
                Console.WriteLine("  {0} Path to line-delimited text file of search terms", "search file".PadRight(13));
                Console.WriteLine("  {0} Path to file containing ignored paths", "ignored paths".PadRight(13));
                Console.WriteLine("  {0} Database server you want to write results to (i.e. UNDEV\\SQLEXPRESS)", "server".PadRight(13));
                Console.WriteLine("  {0} Name of database you want to write results to (i.e. SecSearch)", "database".PadRight(13));
                Console.WriteLine("  {0} Name of table you want to write results to (i.e. CryptoHits)", "table".PadRight(13));
                Console.WriteLine("  {0} File types you want to search (i.e. *.cpp *.c *.h)", "file-spec".PadRight(13));
                return;
            }
            else if (!File.Exists(args[0]))
            {
                Console.WriteLine("\nSearch file doesn't exist");
                return;
            }

            List<string> filespec = new List<string>(args);
            filespec.RemoveRange(0, 5);
            
            SearchHandler h = new SearchHandler(args[0], args[1], args[2], args[3], args[4], filespec.ToArray());
            h.Search(Directory.GetCurrentDirectory());
        }
    }

    class IgnorePath
    {
        public enum Positions { StartsWith, Contains, EndsWith };
        public Positions Position { get; set; }
        public string Path { get; set; }
    }

    class SearchHandler
    {
        Regex rgxTrimmed;
        Regex rgxRegexed;
        Regex rgxChange;
        Regex rgxTeamOwner;
        int fileCount = 0;
        int totalMatchCount = 0;
        SqlConnection connection;
        SqlCommand insert;
        string[] filespec;
        string m_table;
        string winphoneroot = Environment.GetEnvironmentVariable("_WINPHONEROOT");
        string insertTemplate = "INSERT INTO {0} VALUES ('{1}', '{2}', '{3}', {4}, '{5}', '{6}', '{7}', '{8}', '{9}', '')";
        List<IgnorePath> ignoredDirectories;
        List<IgnorePath> ignoredFiles;

        public SearchHandler(string searchTermsFile, string ignoredPathsFile, string databaseServer, string database, string table, string[] fileSpec)
        {
            StringBuilder trimmed = new StringBuilder();
            StringBuilder regexed = new StringBuilder();

            var lines = File.ReadAllLines(searchTermsFile);
            var distinctLines = lines.Select(line => line.Trim()).Distinct();

            foreach (string line in distinctLines)
            {
                if (String.IsNullOrEmpty(line)) continue;
                if (line.StartsWith(";")) continue;
                if (line.EndsWith("("))
                {
                    string trimmedLine = line.TrimEnd('(');
                    trimmed.Append(String.Format("{0}|", trimmedLine + @"\("));
                    regexed.Append(String.Format("{0}{1}|", "(?<![\\w])", trimmedLine + @"\("));
                }
                else
                {
                    trimmed.Append(String.Format("{0}|", line));
                    regexed.Append(String.Format("{0}{1}{2}|", "(?<![\\w])", line, "(?![\\w])"));
                }
            }

            trimmed.Remove(trimmed.Length - 1, 1);
            regexed.Remove(regexed.Length - 1, 1);

            filespec = fileSpec;

            lines = File.ReadAllLines(ignoredPathsFile);
            ignoredDirectories = new List<IgnorePath>();
            ignoredFiles = new List<IgnorePath>();
            foreach (string line in lines)
            {
                var tokens = line.Split();
                string path = Environment.ExpandEnvironmentVariables(tokens[2]).ToUpper();
                var position = (IgnorePath.Positions)Enum.Parse(typeof(IgnorePath.Positions), tokens[1]);
                if (tokens[0] == "Directory")
                {
                    ignoredDirectories.Add(new IgnorePath { Path = path, Position = position });
                }
                else
                {
                    ignoredFiles.Add(new IgnorePath { Path = path, Position = position });
                }
            }

            connection = new SqlConnection(String.Format(@"server={0};Trusted_Connection=yes;database={1};connection timeout=30", databaseServer, database));
            insert = new SqlCommand("", connection);

            m_table = table;

            rgxTrimmed = new Regex(trimmed.ToString(), RegexOptions.Compiled);
            rgxRegexed = new Regex(regexed.ToString(), RegexOptions.Compiled);
            rgxChange = new Regex(@"(?<=\\)[^@]*(?=@)");
            rgxTeamOwner = new Regex("(?<=TEAMOWNER=).*");

            #region Create Table
            // If table doesn't exist, we create it
            {
                connection.Open();

                try
                {
                    string query = @"CREATE TABLE [dbo].[{0}](
                           [Filename] [nvarchar](260) NOT NULL,
                           [Token] [nvarchar](50) NOT NULL,
                           [Line] [nvarchar](max) NOT NULL,
                           [LineNumber] [int] NOT NULL,
                           [Context] [nvarchar](max) NOT NULL,
                           [LastChanged1] [nvarchar](20) NULL,
                           [LastChanged2] [nvarchar](20) NULL,
                           [LastChanged3] [nvarchar](20) NULL,
                           [TechTeam] [nvarchar](50) NULL,
                           [TokenStatus] [nvarchar](50) NULL,
                           CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED 
                           (
                           [Filename] ASC,
                           [LineNumber] ASC
                           )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
                           ) ON [PRIMARY]";
                    SqlCommand createTable = new SqlCommand(String.Format(query, table), connection);
                    createTable.ExecuteNonQuery();
                    Console.WriteLine("Created table {0}", table);
                }
                catch (SqlException ex)
                {
                    // If the table already exists, that's fine, otherwise this exception is bad
                    if (ex.Message != "There is already an object named '" + table + "' in the database.")
                    {
                       throw;
                    }
                }
                
                connection.Close();
            }
            #endregion
        }
        
        // Searches subtree for search terms, and times the whole operation
        public void Search(string path)
        {
            connection.Open();            

            DateTime dt = DateTime.Now;
            SearchSubTree(path);
            double seconds = (DateTime.Now - dt).TotalSeconds;    
            Console.WriteLine("\nSearch took {0} seconds for {1} files ({2} files per second)", seconds, fileCount, fileCount / seconds);
            Console.WriteLine("Found {0} matches", totalMatchCount);

            connection.Close();

            //Console.ReadKey();   
        }

        /// <summary>
        /// Determines whether a path was specified as ignored in the IgnorePaths file
        /// </summary>
        private bool IsPathIgnored(string path, bool isDirectory)
        {
            string upperPath = path.ToUpper();
            var ignoredPaths = isDirectory ? ignoredDirectories : ignoredFiles;
            foreach (var ignorePath in ignoredPaths)
            {
                if ((ignorePath.Position == IgnorePath.Positions.StartsWith && upperPath.StartsWith(ignorePath.Path)) ||
                   (ignorePath.Position == IgnorePath.Positions.Contains && upperPath.Contains(ignorePath.Path)) ||
                   (ignorePath.Position == IgnorePath.Positions.EndsWith && upperPath.EndsWith(ignorePath.Path)))
                {
                    ConsoleColor oldColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Ignoring {0}", path);
                    Console.ForegroundColor = oldColor;
                    return true;
                }
            }
            return false;
        }

        // Given a path, will search all subdirs for regex matches of the search terms.  Will also determine
        // the last 3 aliases to change each file as well.  Writes all data to a specified database.
        private void SearchSubTree(string path)
        {
            if (IsPathIgnored(path, true)) return;

            List<string> files = new List<string>();
            string teamOwner = null;
            foreach (string filetype in filespec)
            {
                files.AddRange(Directory.GetFiles(path, filetype));
            }
            if (files.Count != 0)
            {
                Console.WriteLine("{0} files in {1}", files.Count, path);

                int matchCount = 0;
                foreach (string file in files)
                {
                    fileCount++;

                    if (IsPathIgnored(file, false)) continue;

                    List<SearchMatch> matches = RegExFind(file);
                    if (matches.Count > 0)
                    {
                        matchCount += matches.Count;

                        string[] lastChangers = GetLastChangers(file);
                        if (String.IsNullOrEmpty(teamOwner))
                        {
                            teamOwner = GetTeamOwner(new DirectoryInfo(path));
                        }

                        foreach (SearchMatch match in matches)
                        {
                            insert.CommandText = String.Format(insertTemplate, m_table, match.Filename, match.Token, match.Line, match.LineNumber, match.Context, lastChangers[0], lastChangers[1], lastChangers[2], teamOwner);
                            try
                            {
                                insert.ExecuteNonQuery();
                            }
                            catch (SqlException ex)
                            {
                                // If entry already exists in database we'll discard exception and continue
                                if (ex.Message != String.Format("Violation of PRIMARY KEY constraint 'PK_{0}'. Cannot insert duplicate key in object 'dbo.{0}'.\r\nThe statement has been terminated.", m_table))
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }

                if (matchCount > 0)
                {
                    ConsoleColor oldColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\t({0} matches)", matchCount);
                    Console.ForegroundColor = oldColor;
                    totalMatchCount += matchCount;
                }
            }

            foreach (string dir in Directory.GetDirectories(path))
	        {
                SearchSubTree(dir);
	        }
        }

        // Reads lines from the source file one at a time, matching each one with a simple string find
        // for each entry in the search terms file.  If it matches the simple string search, it then tries to
        // match using the tokenized search terms.  If the regex matches that also, it creates a SearchMatch
        // object.  Returns a List of all the matches.
        private List<SearchMatch> RegExFind(string filename)
        {
            List<SearchMatch> matches = new List<SearchMatch>();
            List<string> lines = new List<string>();

            using (StreamReader sr = new StreamReader(filename))
            {
                while (sr.Peek() != -1)
                {
                    string line = sr.ReadLine();
                    lines.Add(line);
                    if (rgxTrimmed.IsMatch(line))
                    {
                        Match match = rgxRegexed.Match(line);
                        if (match.Success)
                        {
                            StringBuilder context = new StringBuilder();
                            int start = (lines.Count < 5) ? 0 : lines.Count - 5;
                            for (int i = start; i < lines.Count; i++)
                            {
                                context.AppendLine(lines[i]);
                            }
                            matches.Add(new SearchMatch(filename.Replace("'", "''"), match.Value, line.Replace("'", "''"), context.ToString().Replace("'", "''"), lines.Count));
                        }
                    }
                }
            }

            return matches;
        }

        // Uses source depot to determine the last three aliases that changed a source file
        private string[] GetLastChangers(string file)
        {
            string[] changes = new string[3];

            ProcessStartInfo startInfo = new ProcessStartInfo("sd.exe", "changes -i -m 3 " + file);
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;

            int count = 0;
            using (Process p = Process.Start(startInfo))
            {
                while (!p.StandardOutput.EndOfStream)
                {
                    changes[count++] = rgxChange.Match(p.StandardOutput.ReadLine()).Value;
                }
                p.WaitForExit();
            }

            return changes;
        }

        // Walk up the tree until we find a contents.oak file with a populated TEAMOWNER field
        private string GetTeamOwner(DirectoryInfo di)
        {
            var files = di.GetFiles();
            var coaks = files.Where(file => file.Name == "contents.oak");
            if (coaks.Count() == 1)
            {
                var coak = coaks.Single();
                var match = rgxTeamOwner.Match(File.ReadAllText(coak.FullName));
                if (match.Success)
                {
                    return match.Value;
                }
            }
            if (di.Parent.FullName != winphoneroot)
            {
                return GetTeamOwner(di.Parent);
            }
            else
            {
                return null;
            }
        }

        // Holds a single regex match.
        struct SearchMatch
        {
            string m_filename;

            public string Filename
            {
                get { return m_filename; }
                set { m_filename = value; }
            }
            string m_token;

            public string Token
            {
                get { return m_token; }
                set { m_token = value; }
            }
            string m_line;

            public string Line
            {
                get { return m_line; }
                set { m_line = value; }
            }
            string m_context;

            public string Context
            {
                get { return m_context; }
                set { m_context = value; }
            }
            int m_lineNumber;

            public int LineNumber
            {
                get { return m_lineNumber; }
                set { m_lineNumber = value; }
            }

            public SearchMatch(string filename, string token, string line, string context, int lineNumber)
            {
                m_filename = filename;
                m_token = token;
                m_line = line;
                m_context = context;
                m_lineNumber = lineNumber;
            }
        }
    }
}