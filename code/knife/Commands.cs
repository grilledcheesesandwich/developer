//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft
// premium shared source license agreement under which you licensed
// this source code. If you did not accept the terms of the license
// agreement, you are not authorized to use this source code.
// For the terms of the license, please see the license agreement
// signed by you and Microsoft.
// THE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//
#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Threading;
using System.Security;
using Microsoft.Phone.Test.Security.SecurityModel;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;
using System.ComponentModel;
using CommandLineTool;

#endregion

namespace ArmyKnife
{
    class Commands
    {
        public static int Main(string[] args)
        {
            var tool = new CommandLineTool.CommandLineTool() { Name = "Knife", CommandClass = typeof(Commands) };
            return tool.Main(args);
        }


        static uint StringToUint(string val)
        {
            if (val.StartsWith("0x"))
            {
                val = val.Remove(0, 2);
                return UInt32.Parse(val, System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            else
            {
                return UInt32.Parse(val);
            }

        }

        [Documentation("HelloWorld command")]
        public static void HelloWorld(string helloString = "Default")
        {
            Console.WriteLine("Hello World [{0}]", helloString);
        }

        [Documentation("Copy a file")]
        public static void CopyFile(string src, string dst)
        {
            System.IO.File.Copy(src, dst, true);
        }
        
        [Documentation("Remove readonly and system attributes from a file")]
        public static void CleanAttributes(string src)
        {
            FileInfo fi = new FileInfo(src);
            Console.WriteLine("Attributes before set: {0}", fi.Attributes);

            fi.Attributes = FileAttributes.Normal;
            fi.Refresh();

            Console.WriteLine("Attributes after set: {0}", fi.Attributes);
        }

        [Documentation("Delete a file")]
        public static void DeleteFile(string src)
        {
            System.IO.File.Delete(src);
        }

        [Documentation("Open a mutex by name")]
        public static void OpenMutex(string name = "Knife_DefaultMutex")
        {
            Console.WriteLine("*** Attempting to open existing mutex [{0}]", name);

            Mutex mutex = Mutex.OpenExisting(name);
            Console.WriteLine("*** Opened mutex [{0}]", name);

            if (mutex.SafeWaitHandle.IsInvalid) { throw new Exception("Mutex is invalid!"); }
            if (mutex.SafeWaitHandle.IsClosed) { throw new Exception("Mutex is closed!"); }
        }

        [Documentation("Open an event by name")]
        public static void OpenEvent(string name = "Knife_DefaultEvent")
        {
            Console.WriteLine("*** Attempting to open existing event [{0}]", name);

            var kernelEvent = EventWaitHandle.OpenExisting(name);
            Console.WriteLine("*** Opened event [{0}]", name);

            if (kernelEvent.SafeWaitHandle.IsInvalid) { throw new Exception("Event is invalid!"); }
            if (kernelEvent.SafeWaitHandle.IsClosed) { throw new Exception("Event is closed!"); }

            Console.WriteLine("Waiting on event...");
            kernelEvent.WaitOne();
        }

        //[SecuritySafeCritical]
        [Documentation("Open and hold a mutex")]
        public static void HoldMutex(string name = "Knife_DefaultMutex")
        {

            Mutex mutex;

            Console.WriteLine("*** Trying mutex [{0}]", name);
            mutex = new Mutex(false, name);
            Console.WriteLine("*** Created mutex [{0}]",
                name);

            if (mutex.SafeWaitHandle.IsInvalid) { throw new Exception("Mutex is invalid!"); }
            if (mutex.SafeWaitHandle.IsClosed) { throw new Exception("Mutex is closed!"); }


            Console.WriteLine("Holding for snap...");
            Console.ReadLine();
        }

        [Documentation("Create an arbitrary file")]
        public static void CreateFile(string src)
        {
            var fi = File.CreateText(src);
            fi.WriteLine("Hello Worlds");
            fi.Close();
        }

        [Documentation("Write to my profile")]
        public static void WriteRegistryProfile(string currentChamberName)
        {
            Console.WriteLine("Detected execution in {0}", currentChamberName);
            var registryPath = Chambers.GetChamberRegistryLocation(AccessMasks.KEY_WRITE, new Chamber(currentChamberName));

            Console.WriteLine("Verifying registry profile at [{0}]", registryPath);

            var registryApiPath = Path.Combine("HKEY_USERS", registryPath);
            Microsoft.Win32.Registry.SetValue(registryApiPath, "WriteRegistryProfile", "WasHere");
        }


        [Documentation("Create a registry subkey. Only works in HKLM")]
        public static void CreateSubKey(string keyName)
        {
            Registry.LocalMachine.CreateSubKey(keyName);
        }

        [Documentation("Increment or initialize a DWORD in a registry key")]
        public static void IncrementRegistryKey(string [] args)
        {
            int value;
            // string key = @"HKEY_LOCAL_MACHINE\Software\Microsoft";
            // string valueName = "TestInteger";
            string key = args[0];
            string valueName = args[1];

            var result = Registry.GetValue(key, valueName, null);
            if (result == null)
            {
                Console.WriteLine("No value. creating new one.");
                value = 0;
            }
            else
            {
                Console.WriteLine("Read value as {0}", result);
                value = (int)result;
            }

            value++;

            Registry.SetValue(key, valueName, value, RegistryValueKind.DWord);

            //var val = Win32.Registry.LocalMachine.GetValue(hklmValue);
            //Console.WriteLine(@"read {0}\{1}, got value [{2}]", key, valueName, result);
        }

        [Documentation("Hold a file open")]
        public static void HoldFileOpen(string filename, int sleepSeconds = 20)
        {
            
            using (var fw = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                Console.WriteLine("Opened [{0}]. Holding {1} seconds for snap", filename, sleepSeconds);
                Thread.Sleep(sleepSeconds * 1000);
                
            }
            Console.WriteLine("Handle Closed");
        }

        [Abbreviation("rf"), Documentation("Read a file")]
        public static void ReadFile(string path)
        {
            Console.WriteLine(File.ReadLines(path).First());
        }

        [DllImport("secruntime.dll", CharSet = CharSet.Unicode)]
        internal static extern int DeleteSubjectCertificateFromStore(
            byte[] pbData,
            uint cbData,
            string pcwszPassword,
            bool fRemoveFromTCB);

        [DllImport("secruntime.dll", CharSet = CharSet.Unicode)]
        internal static extern int AddSubjectCertificateToStore(
            byte[] pbData,
            uint cbData,
            string pcwszPassword,
            bool fInstallToTCB);

        [Abbreviation("t"), Documentation("Test")]
        public static void Test(string First = "", string Second = "", bool Third = false)
        {
            //var bytes = File.ReadAllBytes(First);
            //int result = AddSubjectCertificateToStore(bytes, (uint)bytes.Length, null, true);
            //if (0 != result)
            //{
            //    throw new Win32Exception(result, String.Format("AddSubjectCertificateToStore failed - 0x{0:X}", result));
            //}

            var certs = new X509Certificate2Collection();
            certs.Import(First);
            var cert = (X509Certificate2)certs[0];

            if (Third)
            {
                Console.WriteLine("foo");
                int result = AddSubjectCertificateToStore(cert.RawData, (uint)cert.RawData.Length, null, true);
                if (0 != result)
                {
                    throw new Win32Exception(result, String.Format("AddSubjectCertificateToStore failed - 0x{0:X}", result));
                }
            }
            else
            {
                Console.WriteLine("bar");
                int result = DeleteSubjectCertificateFromStore(cert.RawData, (uint)cert.RawData.Length, null, true);
                if (0 != result)
                {
                    throw new Win32Exception(result, String.Format("DeleteSubjectCertificateFromStore failed - 0x{0:X}", result));
                }
            }
        }
    }
}
