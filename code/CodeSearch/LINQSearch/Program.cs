using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using System.Xml.Linq;

namespace LINQSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            string databaseServer = @"UNIONDEV\SQLEXPRESS";
            string database = "SecSearch";
            string table = "Paths";
            List<string> queries = new List<string>();
            
            using (SqlConnection connection = new SqlConnection(String.Format(@"server={0};Trusted_Connection=yes;database={1};connection timeout=30", databaseServer, database)))
            {
                SourceSearchDataContext db = new SourceSearchDataContext(connection);

                var results = from f in db.FilesCopies
                              select f;

                var paths = new Dictionary<string, PathToken>();

                Regex rgxRoot = new Regex(@"[^\\]*\\[^\\]*", RegexOptions.Compiled);
                Regex rgxFile = new Regex(@"\\[^\\]*$", RegexOptions.Compiled);
                string insertTemplate = "INSERT INTO {0} VALUES ('{1}', '{2}', '{3}', '{4}', '')";
                string updateTemplate = "UPDATE {0} SET Filenames = Filenames + '{1}, ' WHERE Path = '{2}' AND Token = '{3}'";
                
                foreach (var result in results)
                {
                    string filename = rgxRoot.Replace(result.Filename, "", 1);
                    string path = rgxFile.Replace(filename, "", 1);

                    var pathToken = new PathToken { Path = path, Token = result.Token, Filenames = new List<string>() };

                    if (!paths.Keys.Contains(path+result.Token))
                    {
                        paths.Add(path+result.Token, pathToken);
                        queries.Add(String.Format(insertTemplate, table, path, result.Token, result.TechTeam, (result.TokenStatus != "") ? result.TokenStatus : "OK" ));
                    }

                    if (!paths[path+result.Token].Filenames.Contains(filename))
                    {
                        paths[path+result.Token].Filenames.Add(filename);
                        queries.Add(String.Format(updateTemplate, table, rgxFile.Match(filename), path, result.Token));
                    }
                }
            }

            using (SqlConnection connection = new SqlConnection(String.Format(@"server={0};Trusted_Connection=yes;database={1};connection timeout=30", databaseServer, database)))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("", connection);

                foreach (string query in queries)
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    class PathToken : IComparable<PathToken>
    {
        public string Path { get; set; }
        public string Token { get; set; }
        public List<string> Filenames { get; set; }

        public override bool Equals(Object other)
        {
            return (this.Path == ((PathToken)other).Path) && (this.Token == ((PathToken)other).Token);
        }

        #region IComparable<PathToken> Members

        int IComparable<PathToken>.CompareTo(PathToken other)
        {
            if (this.Path == other.Path)
            {
                return this.Token.CompareTo(other.Token);
            }
            else return this.Path.CompareTo(other.Path);
        }

        #endregion
    }
}