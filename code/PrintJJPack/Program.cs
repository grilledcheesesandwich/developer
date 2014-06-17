using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace PrintJJPack
{
    class Program
    {
        static void Main(string[] args)
        {
            bool unpack = false;
            string bbpack = args[0];
            string outDir = @"\unpacked";

            if (args[0].Equals("unpack", StringComparison.InvariantCultureIgnoreCase))
            {
                unpack = true;
                bbpack = args[1];
                Directory.CreateDirectory(outDir);
            }

            string text = File.ReadAllText(bbpack);

            Regex rgxDepotFile = new Regex(@"(?<=<depotFile>)[^<]*", RegexOptions.Compiled);
            Regex rgxFileContents = new Regex(@"(?<=<FileContents encoding=""base64"">)[^<]*", RegexOptions.Compiled);
            Regex rgxAction = new Regex(@"(?<=<action>)[^<]*", RegexOptions.Compiled);

            MatchCollection matches = rgxFileContents.Matches(text);
            Match match = rgxDepotFile.Match(text);
            Match action = rgxAction.Match(text, match.Index);
            while ("delete" == action.Value)
            {
                match = match.NextMatch();
                action = rgxAction.Match(text, match.Index);
            } 

            foreach (Match encoded in matches)
            {
                string decoded = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(encoded.Value));

                Console.WriteLine(match.Value);

                if (!unpack)
                {
                    Console.WriteLine("\n********************************************************************");
                    Console.WriteLine(decoded);
                    Console.WriteLine("\n********************************************************************");
                }
                else
                {
                    string outPath = Path.Combine(outDir, match.Value.TrimStart('/').Replace('/', '\\'));
                    FileInfo fi = new FileInfo(outPath);
                    if (!fi.Directory.Exists) fi.Directory.Create();
                    File.WriteAllText(outPath, decoded);
                }

                do
	            {
                    match = match.NextMatch();
                    if (0 == match.Index) break;
                    action = rgxAction.Match(text, match.Index);
	            } while ("delete" == action.Value);
            }
            Console.WriteLine(@"Wrote files to \unpacked\...");
        }
    }
}
