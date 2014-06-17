using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Phone.Test.Utilities;
using Microsoft.Phone.Test.Security.SecurityModel;
using Microsoft.Win32;

namespace TestManaged
{
    class Utilities
    {
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DateTime.Now);
            TimeUtilities.SetSystemTime(new DateTime(long.Parse(args[0])));
            Console.WriteLine(DateTime.Now);

            //Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Union", "foobar", "NL:\nCR:\r");

            //Console.WriteLine(TokenManager.GetCurrentToken());
            //using (new DisposableImpersonation(WellKnownChambers.TestChambers.DAC))
            //{
            //    Console.WriteLine(TokenManager.GetCurrentToken());
            //    //string chamberRegistryLocation = Path.Combine("HKU", Chambers.GetChamberRegistryLocation(AccessMasks.KEY_READ, WellKnownChambers.TestChambers.DAC));
            //    Console.WriteLine(Chambers.GetChamberCodePath(WellKnownChambers.TestChambers.DAC));
            //}

            //var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            //store.Open(OpenFlags.ReadWrite);
            //var bytes = File.ReadAllBytes(args[0]);
            //var cert = new X509Certificate2(bytes);
            //store.Add(cert);

            //foreach (var cer in store.Certificates)
            //{
            //    Console.WriteLine(cer.Subject);
            //}
            //store.Remove(cert);

            //Directory.GetFiles(@"\Windows", "*.dll").Where(file => Utilities.IsModule(file)).OrderBy(p => p).ToList().ForEach(file => Console.WriteLine(file));
        }
    }
}
