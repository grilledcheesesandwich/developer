using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using MS.SDWrapper;

namespace FixAdds
{
    class Program
    {
        static Regex rgxBuildError = new Regex(@"\d+ Warnings,  0 Errors");
        static Client client = new Client(Environment.GetEnvironmentVariable("_WINCEROOT"));
        static ConsoleColor oldBackground = Console.BackgroundColor;
        static ConsoleColor oldForeground = Console.ForegroundColor;

        static bool Edit(string path)
        {
            Command edit = new Command("edit");
            edit.Arguments.Add(path);
            CommandResults cr = client.Execute(edit);

            // If it has InfoOutput, checkout succeeded
            if (cr.HasInfoOutput)
            {
                Console.WriteLine(path + " checked out");
                return true;
            }

            // If it doesn't have ErrorOutput and it has WarningOutput, it MIGHT have succeeded
            if (!cr.HasErrorOutput && cr.HasWarningOutput)
            {
                foreach (CommandOutput co in cr.WarningOutput.Items)
                {
                    if (co.Message.Contains("already opened"))
                    {
                        Console.WriteLine(path + " checked out");
                        return true;
                    }
                }
            }

            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Couldn't check out " + path);
            Console.BackgroundColor = oldBackground;
            Console.ForegroundColor = oldForeground;
            return false;
        }

        static string Build()
        {
            ProcessStartInfo psi = new ProcessStartInfo("build.exe", "-c");
            psi.UseShellExecute = false;
            using (Process p = Process.Start(psi))
            {
                p.WaitForExit();
            }
            string buildOutput = File.ReadAllText("build.log");
            if (!rgxBuildError.IsMatch(buildOutput))
            {
                throw new Exception("Build.exe at " + Environment.CurrentDirectory + " failed");
            }
            return buildOutput;
        }

        static void Main(string[] args)
        {
            // If obsolete warning isn't set, build new dll with it set
            {
                string localDir = Path.Combine(Environment.GetEnvironmentVariable("_PRIVATEROOT"), @"test\common\managed\Logger");
                string localPath = Path.Combine(localDir, @"LogResultManager.cs");
                string sharePath = @"\\uniondev\Shared\FixAdds\LogResultManager.cs";

                if (File.GetLastWriteTimeUtc(localPath) != File.GetLastWriteTimeUtc(sharePath))
                {
                    Command opened = new Command("opened");
                    opened.Arguments.Add(localPath);
                    CommandResults cr = client.Execute(opened);
                    if (!cr.HasWarningOutput)
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(localPath + " is already opened");
                        Console.BackgroundColor = oldBackground;
                        Console.ForegroundColor = oldForeground;
                        return;
                    }

                    if (!Edit(localPath)) return;
                    File.Copy(sharePath, localPath, true);
                    string oldCurrentDirectory = Environment.CurrentDirectory;
                    Environment.CurrentDirectory = localDir;
                    Build();
                    Environment.CurrentDirectory = oldCurrentDirectory;
                }
            }

            string buildOutput = Build();

            var files = new Dictionary<string, List<int>>();
            Regex rgxAddWarning = new Regex(@"(\w+\.cs)\((\d+).*is obsolete: 'Please use the \.Add\(bool, string\[, params object\[\] args\]\) functions");

            foreach (Match match in rgxAddWarning.Matches(buildOutput))
            {
                string filename = match.Groups[1].Value;
                int lineNumber = Convert.ToInt32(match.Groups[2].Value) - 1;

                if (!files.ContainsKey(filename)) files.Add(filename, new List<int>());
                files[filename].Add(lineNumber);
            }

            Regex rgxFixAdd = new Regex(@"(?<=\.Add\()([\s@]*\""[^""]*""),\s*(.*)(?=\);)");

            foreach (var filePair in files)
            {
                string fileName = filePair.Key;
                string fileText = File.ReadAllText(fileName);
                string[] lines = fileText.Split('\n');
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(fileName);
                foreach (var lineNumber in filePair.Value)
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("{0}:BEFORE: {1}", lineNumber, lines[lineNumber]);
                    if (rgxFixAdd.IsMatch(lines[lineNumber]))
                    {
                        lines[lineNumber] = rgxFixAdd.Replace(lines[lineNumber], "$2, $1");
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.WriteLine("{0}:AFTER: {1}", lineNumber, lines[lineNumber]);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("Couldn't successfully parse above line");
                    }
                    Console.BackgroundColor = oldBackground;
                    Console.ForegroundColor = oldForeground;
                }
                fileText = string.Join("\n", lines);

                FileInfo fi = new FileInfo(fileName);
                if (fi.IsReadOnly && !Edit(fi.FullName)) return;
                string prefix = fileName.Substring(0, fileName.LastIndexOf('.'));
                File.Copy(fileName, prefix + "_old" + fi.Extension, true);
                File.WriteAllText(fileName, fileText);
            }
        }
    }
}
