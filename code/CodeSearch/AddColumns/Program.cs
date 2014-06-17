using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.IO;

namespace AddColumns
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines("BadCryptoAlgs2.csv");

            string databaseServer = @"UNIONDEV\SQLEXPRESS";
            string database = "SecSearch";
            SqlConnection connection = new SqlConnection(String.Format(@"server={0};Trusted_Connection=yes;database={1};connection timeout=30", databaseServer, database));
            connection.Open();

            string command = "UPDATE Files SET TokenStatus = '{0}' WHERE token = '{1}'";
            SqlCommand update = new SqlCommand("", connection);

            foreach (string line in lines)
            {
                string token = line.Split(',')[0].Trim();
                string status = line.Split(',')[2].Trim();
                update.CommandText = String.Format(command, status, token);
                int rowsAffected = update.ExecuteNonQuery();
            }

            connection.Close();
        }

        //static void Main(string[] args)
        //{
        //    string[] lines = File.ReadAllLines("TechTeams.csv");

        //    string databaseServer = @"UNIONDEV\SQLEXPRESS";
        //    string database = "SecSearch";
        //    SqlConnection connection = new SqlConnection(String.Format(@"server={0};Trusted_Connection=yes;database={1};connection timeout=30", databaseServer, database));
        //    connection.Open();

        //    string command = "UPDATE Files SET TechTeam = '{0}' WHERE filename LIKE '%{1}%'";
        //    SqlCommand update = new SqlCommand("", connection);

        //    //Regex parse = new Regex("(?=

        //    foreach (string line in lines)
        //    {
        //        string path = line.Split(',')[0];
        //        path = path.Replace(@"D:\Seven\", "");
        //        update.CommandText = String.Format(command, line.Split(',')[1], path);
        //        int rowsAffected = update.ExecuteNonQuery();
        //    }

        //    connection.Close();
        //}
    }
}
