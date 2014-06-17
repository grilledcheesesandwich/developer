using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace ConvertTabs
{
    class Program
    {
        static void Main(string[] args)
        {
            if (1 == args.Length && args[0].Contains("?"))
            {
                Console.WriteLine("Converts tabs to four spaces in all files matching filespec in this directory and all sub-directories");
                Console.WriteLine("\nCONVERTTABS [filespec]");
                Console.WriteLine("\nfilespec\tOptional list of file filters, for example:");
                Console.WriteLine("\t\tCONVERTTABS *.c* *.h* sources");
                Console.WriteLine("\nIf filespec not specified, will default to: \"sources *.cs *.cpp *.c *.h *.hxx *.xml\"");
                
                return;
            }

            if (0 == args.Length)
            {
                args = new string[] { "sources", "*.cs", "*.cpp", "*.c", "*.h", "*.hxx", "*.xml" };
            }

            List<string> files = new List<string>();
            foreach (string filetype in args)
            {
                files.AddRange(Directory.GetFiles(Environment.CurrentDirectory, filetype, SearchOption.AllDirectories));
            }

            Encoding[] encodings = { Encoding.BigEndianUnicode, Encoding.Unicode, Encoding.UTF8 };

            ProcessStartInfo psi = new ProcessStartInfo("sd.exe");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            foreach (var file in files)
            {
                string contents = File.ReadAllText(file);
                if (contents.Contains("\t"))
                {
                    Console.WriteLine("De-tabifying {0}", file);
                    
                    // Check out file (if it's already checked out nothing will happen)
                    psi.Arguments = "edit " + file;
                    using (Process p = Process.Start(psi))
                    {
                        p.WaitForExit();
                    }
                    
                    // Determine encoding of file before conversion
                    UInt16 bom;
                    using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
                    {
                        bom = br.ReadUInt16();
                    }
                    Encoding enc = Encoding.ASCII;
                    foreach (var encoding in encodings)
	                {
                        var bytes = encoding.GetPreamble();
                        var preamble = BitConverter.ToUInt16(bytes, 0);
                		if (bom == preamble)
                        {
                            enc = encoding;
                        }
	                }

                    // Write out converted file while preserving original encoding
                    File.WriteAllText(file, contents.Replace("\t", "    "), enc);
                }
            }
        }
    }
}
