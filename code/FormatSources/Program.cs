using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace FormatSources
{
    class Program
    {
        static void Main(string[] args)
        {
            string sources = File.ReadAllText(args[0]);

            Regex rgxReferences = new Regex(@"(;|MANAGED_REFERENCES=)([^;]*)");

            string sourcesOut = rgxReferences.Replace(sources, "MANAGED_REFERENCES=$(MANAGED_REFERENCES);$2\n", -1, sources.IndexOf("MANAGED_REFERENCES"));

            File.WriteAllText(args[0], sourcesOut);
        }
    }
}
