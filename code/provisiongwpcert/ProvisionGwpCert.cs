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

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Phone.Test.Utilities;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace ProvisionGwpCert
{
    public static class Utilities
    {
        public static X509Certificate2Collection FindByEnhancedKeyUsage(this X509Store store, string oid)
        {
            X509Certificate2Collection certs = new X509Certificate2Collection();
            foreach (var cert in store.Certificates)
            {
                foreach (var ekuOid in cert.GetEkuOids())
                {
                    if (ekuOid == oid)
                    {
                        certs.Add(cert);
                        break;
                    }
                }
            }
            return certs;
        }

        public static IEnumerable<string> GetEkuOids(this X509Certificate2 cert)
        {
            uint CERT_FIND_EXT_ONLY_ENHKEY_USAGE_FLAG = 0x2;
            const uint CRYPT_E_NOT_FOUND = 0x80092004;
            uint pcbUsage;
            IntPtr pUsage = IntPtr.Zero;
            if (!NativeMethods.CertGetEnhancedKeyUsage(cert.Handle, CERT_FIND_EXT_ONLY_ENHKEY_USAGE_FLAG, pUsage, out pcbUsage))
            {
                uint error = (uint)Marshal.GetLastWin32Error();
                if (error == CRYPT_E_NOT_FOUND)
                {
                    yield break;
                }
                ErrorUtilities.ThrowWin32Exception("CertGetEnhancedKeyUsage");
            }
            if (pcbUsage > 0)
            {
                pUsage = MarshalEx.AllocHGlobal((int)pcbUsage);
                if (!NativeMethods.CertGetEnhancedKeyUsage(cert.Handle, CERT_FIND_EXT_ONLY_ENHKEY_USAGE_FLAG, pUsage, out pcbUsage))
                {
                    ErrorUtilities.ThrowWin32Exception("CertGetEnhancedKeyUsage");
                }
                var usage = (NativeMethods.CERT_ENHKEY_USAGE)Marshal.PtrToStructure(pUsage, typeof(NativeMethods.CERT_ENHKEY_USAGE));
                var intPtrs = new int[usage.cUsageIdentifier];
                Marshal.Copy(usage.rgpszUsageIdentifier, intPtrs, 0, (int)usage.cUsageIdentifier);
                foreach (var intPtr in intPtrs)
                {
                    IntPtr stringPointer = new IntPtr(intPtr);

                    yield return Marshal.PtrToStringAnsi(stringPointer);
                }
                MarshalEx.FreeHGlobal(pUsage);
            }
        }
    }

    public static class Program
    {
        static void Main(string[] args)
        {
            RemoveCerts();

            GwpCertProvisionCreateWrapper();
            GwpCertProvisionPrepareWrapper();
            GwpCertProvisionCommitWrapper();
            GwpCertProvisionCloseWrapper();
        }

        static IntPtr hGwpCP = IntPtr.Zero;
        static X509Certificate2 GWPCert = null;

        //subject name
        static string prefixForInt = "CN=urn:wp-ac-hash-2:";
        static string prefixForProd = "CN=urn:wp-ac-hash-2:";
        static string RootCASubNameINT = "CN=Microsoft Testing Root Certificate Authority, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";
        static string RootCASubNameProd = "CN=Microsoft Root Certificate Authority, DC=microsoft, DC=com";
        //EKU
        static string EKUForINT = "1.3.6.1.4.1.311.71.1.1";
        static string EKUForProd = "1.3.6.1.4.1.311.71.1.2";
        static string ClientAuthenticationEKU = "1.3.6.1.5.5.7.3.2";

        //Activation Codes

        // These are hardcoded to return certain error codes on the server, keep in sync with server
        static public string ActivationCode400 = "BM6Y7-8749R-J4D22-RWW2Q-MDR9C";
        static public string ActivationCode403 = "KC8FT-F26KD-Q8PBP-GTB6F-HHKCH";
        static public string ActivationCode500 = "P69T4-YMMGY-VGH4P-C37MR-P7QDJ";
        static public string ActivationCode404 = "6CKC7-R4JQY-R97C6-WT2YQ-TJYP3";
        static public string ActivationCode409 = "7MC7J-RJ7DG-PC6G9-X48C2-CYHBM";
        static public string ActivationCode503 = "7M6VP-GT8W8-YCDQJ-FJF9Y-7KHQX";

        public static string AppContainerCertPath = @"C:\Data\Users\DefaultAppAccount\AppData\local\packages\DAC\ac\Microsoft\SystemCertificates";

        // Registry
        static string GwpCPCRegistryPath = @"SOFTWARE\MICROSOFT\GWPCPC";
        static string GwpCPCRegistryFullPath = @"HKEY_LOCAL_MACHINE\" + GwpCPCRegistryPath;
        static string IntEndpointRegValue = "IntEndpoint";
        static string ProdEndpointRegValue = "ProdEndpoint";

        static string GwpPvkRegFullPath = @"HKEY_LOCAL_MACHINE\System\PVK";
        static string ACRegValue = "ActivationCode";

        // Valid codes from TEST bucket. Redemption limit is 2000. They have a SQL script that resets redeemed count on a weekly basis.  Keep in sync with server
        private static string[] activationCodes = new string[]{
                                                    "36KRG-V43TV-78B9B-YGPJ8-MB6K9",
                                                    "MD2CV-3HWBT-J2WPQ-PM4WY-YXB4T",
                                                    "GJFT2-TVJ93-H8RW6-R7M92-962GD",
                                                    "324HP-XVFMF-RC9KQ-9CQQ6-VYK9P",
                                                    "6KF8Q-MVDPP-DCQPR-48WR3-P9TB8",
                                                    "BF6CR-BC888-WW634-46BTY-9BQ3J",
                                                    "PVTFK-VM3G9-2WFFF-G7CVT-4DW8W",
                                                    "MQDQ8-7GTVV-3TJK7-W4GWY-6FDD2",
                                                    "6PHHR-RVTFK-TQDDP-Y7JY3-YQMKB",
                                                    "GM7XH-2WW76-JDPXW-DWKQF-M8JPK",
                                                    "7Q2XW-YYYYQ-4MDJT-QT9JY-Y37HV"};


        public static string[] ActivationCodes { get { return activationCodes; } }

        public static string ValidActivationCode { get { return activationCodes[0]; } }

        static public void GwpCertProvisionCreateWrapper()
        {
            IntPtr tempGwpCP = IntPtr.Zero;
            int hResult = NativeMethods.GwpCertProvisionCreate(out tempGwpCP);
            if (NativeMethods.Failed(hResult) || (tempGwpCP == IntPtr.Zero))
            {
                ErrorUtilities.ThrowWin32Exception(hResult, "GwpCertProvisionCreate");
            }
            hGwpCP = tempGwpCP;
        }

        static public void GwpCertProvisionPrepareWrapper()
        {
            int hResult = NativeMethods.GwpCertProvisionPrepare(hGwpCP);
            if (NativeMethods.Failed(hResult))
            {
                ErrorUtilities.ThrowWin32Exception(hResult, "GwpCertProvisionPrepare");
            }
        }

        static public void GwpCertProvisionCommitWrapper(string activationCode)
        {
            int hResult = NativeMethods.GwpCertProvisionCommit(hGwpCP, activationCode);
            if (NativeMethods.Failed(hResult))
            {
                ErrorUtilities.ThrowWin32Exception(hResult, "GwpCertProvisionCommit");
            }
        }

        static public void GwpCertProvisionCommitWrapper()
        {
            int hResult = NativeMethods.GwpCertProvisionCommit(hGwpCP, null);
            if (NativeMethods.Failed(hResult))
            {
                ErrorUtilities.ThrowWin32Exception(hResult, "GwpCertProvisionCommit");
            }
        }

        static public int GwpCertProvisionCommit(string activationCode)
        {
            return NativeMethods.GwpCertProvisionCommit(hGwpCP, activationCode);
        }

        static public void GwpCertProvisionCloseWrapper()
        {
            if (hGwpCP != IntPtr.Zero)
            {
                int hResult = NativeMethods.GwpCertProvisionClose(hGwpCP);
                if (NativeMethods.Failed(hResult))
                {
                    ErrorUtilities.ThrowWin32Exception(hResult, "GwpCertProvisionClose");
                }
                hGwpCP = IntPtr.Zero;
            }
        }


        internal static void RemoveCerts()
        {
            // Clean SYSTEM's MY stores
            {
                X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                var prodCerts = store.FindByEnhancedKeyUsage(EKUForProd);
                store.RemoveRange(prodCerts);
                var intCerts = store.FindByEnhancedKeyUsage(EKUForINT);
                store.RemoveRange(intCerts);
            }

            // Clean SYSTEM's CA store
            {
                X509Store store = new X509Store(StoreName.CertificateAuthority, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadWrite);
                var prodCerts = store.FindByEnhancedKeyUsage(EKUForProd);
                store.RemoveRange(prodCerts);
                var intCerts = store.FindByEnhancedKeyUsage(EKUForINT);
                store.RemoveRange(intCerts);
            }

            // Clean up the test app container (DAC) certs
            if (Directory.Exists(AppContainerCertPath)) { Directory.Delete(AppContainerCertPath, true); }

            // Delete stored GWPCertInfo blob
            string path = Path.Combine(GwpCPCRegistryPath, "Production");
            var blobKey = Registry.LocalMachine.OpenSubKey(path, true);
            if (blobKey != null)
            {
                blobKey.DeleteValue("GWPCertInfo", false);
            }
            
            path = Path.Combine(GwpCPCRegistryPath, "Test");
            blobKey = Registry.LocalMachine.OpenSubKey(path, true);
            if (blobKey != null)
            {
                blobKey.DeleteValue("GWPCertInfo", false);
            }           
        }
    }
}
