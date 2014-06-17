using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FindFunctions
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText(args[0]);

            List<string> files = new List<string>(Directory.GetFiles(Environment.CurrentDirectory, "*.c*", SearchOption.AllDirectories));
            List<string> fileContents = new List<string>(files.Count);
            foreach (string file in files)
            {
                fileContents.Add(File.ReadAllText(file));
            }

            MatchCollection matches = Regex.Matches(text, @"^(static)?\s*\w+(\s+|\s*\*\s*)\w+\([^\)]*\)\s*{", RegexOptions.Multiline);
            foreach (Match match in matches)
            {
                string func = match.Value;
                func = Regex.Match(func, @"\w+(?=\()", RegexOptions.Compiled).Value;
                int count = 0;
                foreach (string fileContent in fileContents)
                {
                    count += Regex.Matches(fileContent, String.Format(@"\b{0}\b", func)).Count;
                }
                Console.WriteLine("{0}\t{1}", func, count);
            }
        }
    }
}