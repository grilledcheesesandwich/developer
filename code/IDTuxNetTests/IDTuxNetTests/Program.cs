using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace IDTuxNetTests
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("File not found");
                return;
            }

            string file = File.ReadAllText(args[0]);
            string oldFile = file;

            Regex rgxAttribute = new Regex(@"[\[\.]TestCaseAttribute[^\]]*\]");
            Regex rgxID = new Regex(@"(?<=,\s*ID\s*=\s*)\d+(?=\s*,)");
            Regex rgxNoID = new Regex(@"(?<=""\s*),");

            int count = 0;
            int position = 0;
            Match match = rgxAttribute.Match(file, position);
            while (match.Success)
            {
                count++;
                if (rgxID.IsMatch(match.Value))
                {
                    file = rgxID.Replace(file, count.ToString(), 1, match.Index);
                    position = rgxID.Match(file, match.Index).Index;
                }
                else
                {
                    file = rgxNoID.Replace(file, String.Format(", ID = {0},", count), 1, match.Index);
                    position = rgxNoID.Match(file, match.Index).Index;
                }
                match = rgxAttribute.Match(file, position);
            }

            if (file != oldFile)
            {
                string old = args[0].Insert(args[0].LastIndexOf('.'), "_old");
                File.Copy(args[0], old, true);
                File.WriteAllText(args[0], file);
                Console.WriteLine("Made {0} changes.  Backup written to {1}", count, old);
            }
            else
            {
                Console.WriteLine("No changes were made");
            }
        }
    }
}
