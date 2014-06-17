using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.IO;

namespace DBAnalyze
{
    class Program
    {
        static List<string> files = new List<string>();

        static void Main(string[] args)
        {
            string databaseServer = @"UNIONDEV\SQLEXPRESS";
            string database = "Test";
            SqlConnection connection = new SqlConnection(String.Format(@"server={0};Trusted_Connection=yes;database={1};connection timeout=30", databaseServer, database));
            connection.Open();

            File.Delete("owners.txt");

            WriteDirs("SELECT DISTINCT filename FROM Files WHERE filename LIKE 'd:\\seven%'", connection);
            WriteDirs("SELECT DISTINCT filename FROM Files WHERE filename LIKE 'd:\\04%'", connection);
            WriteDirs("SELECT DISTINCT filename FROM Files WHERE filename LIKE 'd:\\bowmore%'", connection);
        }

        static void WriteDirs(string query, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand(query, connection);
            using (SqlDataReader sdr = select.ExecuteReader())
            {
                using (StreamWriter sw = new StreamWriter("owners.txt", true))
                {
                    sw.WriteLine("\n" + query.Split('\'')[1] + "\n");

                    foreach (DbDataRecord field in sdr)
                    {
                        string rgxDirString = @"(?<=\w:\\[^\\]*\\)[^\\]*\\[^\\]*\\[^\\]*\\[^\\]*\\";
                        string rgxPathString = @"[^\\]*\\[^\\]*\\[^\\]*\\[^\\]*\\[^\\]*\\[^\\]*\\";
                        string file = field["filename"].ToString();

                        Match matchDir;
                        Match matchPath;

                        do
                        {
                            Regex rgxDir = new Regex(rgxDirString);
                            Regex rgxPath = new Regex(rgxPathString);
                            matchDir = rgxDir.Match(file);
                            matchPath = rgxPath.Match(file);
                            rgxDirString = rgxDirString.Remove(rgxDirString.Length - 8);
                            rgxPathString = rgxPathString.Remove(rgxPathString.Length - 8);
                        } while (matchDir.Success == false || matchPath.Success == false);

                        string directory = matchDir.Value;
                        string path = matchPath.Value;

                        if (!files.Contains(directory))
                        {
                            files.Add(directory);

                            string coak = Path.Combine(path, "contents.oak");
                            if (File.Exists(coak))
                            {
                                string[] lines = File.ReadAllLines(coak);
                                directory = directory.PadRight(60);
                                foreach (string line in lines)
                                {
                                    if (line.IndexOf("owner", StringComparison.OrdinalIgnoreCase) != -1)
                                        directory += line + ' ';
                                }
                            }

                            sw.WriteLine(directory);
                        }
                    }
                }
            }
        }
    }
}
