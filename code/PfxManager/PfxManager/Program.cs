using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

namespace PfxManager
{
    class Program
    {
        private static void Main(string[] args)
        {
            string currentDirectory;
            bool addCerts = true;
            if ((1 == args.Length) && ("add" == args[0]))
            {
                addCerts = true;
            }
            else if ((1 == args.Length) && ("remove" == args[0]))
            {
                addCerts = false;
            }
            else
            {
                Console.WriteLine("USAGE: PfxManager [add | remove]");
                Console.WriteLine("  add       Adds TCB, Elevated, Standard, Restricted Rights certs to system store");
                Console.WriteLine("  remove    Removes TCB, Elevated, Standard, Restricted Rights certs from system store");
                Console.WriteLine();
                Console.WriteLine("  Either run from a build window or from the directory containing the pfx files");
                return;
            }
            try
            {
                currentDirectory = Path.Combine(Environment.GetEnvironmentVariable("_PUBLICROOT"), @"common\oak\signkeys");
            }
            catch (ArgumentNullException)
            {
                currentDirectory = Environment.CurrentDirectory;
                List<string> pfxs = new List<string>(Directory.GetFiles(currentDirectory, "*.pfx"));
                if (!pfxs.Exists(file => file.EndsWith("tcb.pfx", StringComparison.CurrentCultureIgnoreCase)) ||
                    !pfxs.Exists(file => file.EndsWith("elevated.pfx", StringComparison.CurrentCultureIgnoreCase)) ||
                    !pfxs.Exists(file => file.EndsWith("tcb.pfx", StringComparison.CurrentCultureIgnoreCase)) ||
                    !pfxs.Exists(file => file.EndsWith("tcb.pfx", StringComparison.CurrentCultureIgnoreCase)))
                {
                    Console.WriteLine("ERROR: Must run from a build window or directory containing PFX files");
                    return;
                }
            }
            string tcb = Path.Combine(currentDirectory, "tcb.pfx");
            string elevated = Path.Combine(currentDirectory, "elevated.pfx");
            string standard = Path.Combine(currentDirectory, "standard.pfx");
            string restricted = Path.Combine(currentDirectory, "restricted.pfx");

            X509Certificate2Collection certs = new X509Certificate2Collection();
            certs.Import(tcb, null, X509KeyStorageFlags.Exportable);
            certs.Import(elevated, null, X509KeyStorageFlags.Exportable);
            certs.Import(standard, null, X509KeyStorageFlags.Exportable);
            certs.Import(restricted, null, X509KeyStorageFlags.Exportable);
            
            X509Certificate2Collection rootCerts = certs.Find(X509FindType.FindBySubjectName, "Root", false);
            X509Certificate2Collection otherCerts = certs.Find(X509FindType.FindBySubjectName, "CA", false);
            X509Certificate2Collection myCerts = certs.Find(X509FindType.FindBySubjectName, "Signing Cert", false);

            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            X509Store root = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            X509Store other = new X509Store(StoreName.AddressBook, StoreLocation.CurrentUser);
            my.Open(OpenFlags.ReadWrite);
            root.Open(OpenFlags.ReadWrite);
            other.Open(OpenFlags.ReadWrite);

            X509Store store = new X509Store(StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);

            // If we're adding the certs and the root cert isn't already in the store OR
            // If we're removing the certs and the root cert IS already in the store, start click thread
            if (!addCerts && root.Certificates.Contains(rootCerts[0]) ||
                addCerts && !root.Certificates.Contains(rootCerts[0]))
            {
                System.Threading.Thread clickThread = new System.Threading.Thread(ClickYes);
                clickThread.Start();
            }

            if (addCerts)
            {
                //store.AddRange(certs);
                root.AddRange(rootCerts);
                my.AddRange(myCerts);
                other.AddRange(otherCerts);
            }
            else
            {
                //store.RemoveRange(certs);
                root.RemoveRange(rootCerts);
                my.RemoveRange(myCerts);
                other.RemoveRange(otherCerts);
            }
        }

        private static void ClickYes()
        {
            using (Process p = Process.GetProcessesByName("csrss")[0])
            {
                while (p.MainWindowTitle == "") p.Refresh();
            }
            System.Windows.Forms.SendKeys.SendWait("Y");
        }
    }
}
