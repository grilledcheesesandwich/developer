using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace TailResults
{
    class Program
    {
        static void Main(string[] args)
        {
            string frd = Environment.GetEnvironmentVariable("_FLATRELEASEDIR");
            List<string> results = new List<string>();
            NumericComparer nc = new NumericComparer();

            while (true)
            {
                results = new List<string>(Directory.GetFiles(frd, "results*.log"));
                results.Sort(nc);
                if (results.Count > 0) break;
                System.Threading.Thread.Sleep(1000);
            }

            string lastResult = results[results.Count - 1];

            ProcessStartInfo psi = new ProcessStartInfo("tail.exe", "-f " + lastResult);
            psi.UseShellExecute = false;

            while (true)
            {
                Console.WriteLine('\n' + new string('-', lastResult.Length));
                Console.WriteLine(lastResult);
                Console.WriteLine(new string('-', lastResult.Length) + '\n');
                using (Process p = Process.Start(psi))
                {
                    do
                    {
                        results = new List<string>(Directory.GetFiles(frd, "results*.log"));
                        results.Sort(nc);
                        System.Threading.Thread.Sleep(1000);
                    } while (lastResult == (0 == results.Count ? null : results[results.Count - 1]));
                    lastResult = results[results.Count - 1];
                    p.Kill();
                }
                psi.Arguments = "-f " + lastResult;
            }
        }
    }

    public class NumericComparer : IComparer<string>
    {
        static Regex rgxResults = new Regex(@"(?<=results)\d*(?=\.)", RegexOptions.Compiled);

        public NumericComparer()
        { }

        public int Compare(string x, string y)
        {
            string left = rgxResults.Match(x).Value;
            string right = rgxResults.Match(y).Value;
            int leftInt = string.IsNullOrEmpty(left) ? 0 : Int32.Parse(left);
            int rightInt = string.IsNullOrEmpty(right) ? 0 : Int32.Parse(right);
            return leftInt.CompareTo(rightInt);
        }
    }
}
