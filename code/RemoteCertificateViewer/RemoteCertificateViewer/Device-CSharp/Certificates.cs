using System;
//using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace RemoteCertificateViewer
{
    static class NativeMethods
    {
        internal const int CERT_STORE_ADD_REPLACE_EXISTING = 3;
        internal const int X509_ASN_ENCODING = 0x00000001;
        internal const int CERT_STORE_PROV_SYSTEM = 10;
        internal const int CERT_SYSTEM_STORE_LOCAL_MACHINE = 131072;
        internal const uint CERT_FIND_HASH = 0x10000;

        [DllImport("crypt32.dll", EntryPoint = "CertOpenStore", CharSet = CharSet.Auto)]
        internal static extern IntPtr _CertOpenStore(
            IntPtr lpszStoreProvider,
            int dwMsgAndCertEncodingType,
            IntPtr hCryptProv,
            uint dwFlags,
            string pvPara);

        internal static IntPtr CertOpenLMStore(string storeName)
        {
            IntPtr hCertStore;
            hCertStore = _CertOpenStore(
                (IntPtr)CERT_STORE_PROV_SYSTEM,
                0,
                IntPtr.Zero,
                CERT_SYSTEM_STORE_LOCAL_MACHINE,
                storeName);

            if (hCertStore == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "CertOpenStore");
            }

            return hCertStore;
        }

        [DllImport("crypt32.dll", EntryPoint = "CertOpenSystemStore", CharSet = CharSet.Auto)]
        private extern static IntPtr _CertOpenSystemStore(IntPtr hprov,
            string szSubsystemProtocol);

        internal static IntPtr CertOpenSystemStore(string szStoreName)
        {
            IntPtr hCertStore = _CertOpenSystemStore(IntPtr.Zero, szStoreName);

            if (hCertStore == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "CertOpenSystemStore");
            }

            return hCertStore;
        }

        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern int CertAddCertificateContextToStore(
            IntPtr hCertStore,
            IntPtr pCertContext,
            int dwAddDisposition,
            IntPtr ppStoreContext);

        [DllImport("crypt32.dll", SetLastError = true)]
        internal static extern int CertDeleteCertificateFromStore(
            IntPtr pCertContext);

        [DllImport("crypt32.dll", SetLastError = true)]
        public static extern bool CertCloseStore(
            IntPtr hCertStore,
            uint dwFlags);

        [DllImport("Crypt32.dll", SetLastError = true)]
        public static extern IntPtr CertFindCertificateInStore(
            IntPtr hCertStore,
            uint dwCertEncodingType,
            uint dwFindFlags,
            uint dwFindType,
            ref CRYPTOAPI_BLOB pvFindPara,
            IntPtr pPrevCertContext
        );

        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_CONTEXT
        {
            public int dwCertEncodingType;
            public IntPtr pbCertEncoded;
            public int cbCertEncoded;
            public IntPtr pCertInfo;
            public IntPtr hCertStore;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPTOAPI_BLOB
        {
            public uint cbData;
            public IntPtr pbData;
        }
    }

    static class Extensions
    {
        public static void Add(this X509Store store, X509Certificate2 cert)
        {
            IntPtr hCertStore;

            Console.WriteLine("CAPI2: Installing cert {0}", cert.GetCertHashString());

            // open store
            if (StoreLocation.LocalMachine == store.Location)
            {
                hCertStore = NativeMethods.CertOpenLMStore(store.Name);
            }
            else
            {
                hCertStore = NativeMethods.CertOpenSystemStore(store.Name);
            }

            int result = NativeMethods.CertAddCertificateContextToStore(hCertStore,
                cert.Handle,
                NativeMethods.CERT_STORE_ADD_REPLACE_EXISTING,
                IntPtr.Zero);
            if (result == 0)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), String.Format("CertAddCertificateContextToStore ({0:x)}", Marshal.GetLastWin32Error()));
            }

            // clean up store handle
            NativeMethods.CertCloseStore(hCertStore, 0);
        }

        public static void Remove(this X509Store store, X509Certificate2 cert)
        {
            IntPtr hCertStore;

            // open store
            if (StoreLocation.LocalMachine == store.Location)
            {
                hCertStore = NativeMethods.CertOpenLMStore(store.Name);
            }
            else
            {
                hCertStore = NativeMethods.CertOpenSystemStore(store.Name);
            }

            // CRYPT_HASH_BLOB contains an embedded pointer, need to pass this as an IntPtr
            byte[] hashBytes = cert.GetCertHash();
            NativeMethods.CRYPTOAPI_BLOB hashBlob;
            IntPtr pBytes = Marshal.AllocHGlobal(hashBytes.Length);
            Marshal.Copy(hashBytes, 0, pBytes, hashBytes.Length);
            hashBlob.pbData = pBytes;
            hashBlob.cbData = (uint)hashBytes.Length;

            IntPtr hContext = NativeMethods.CertFindCertificateInStore(hCertStore,
                NativeMethods.X509_ASN_ENCODING,
                0,
                NativeMethods.CERT_FIND_HASH,
                ref hashBlob,
                IntPtr.Zero);

            Marshal.FreeHGlobal(pBytes);

            int result = NativeMethods.CertDeleteCertificateFromStore(hContext);
            if (result == 0)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), String.Format("CertDeleteCertificateFromStore ({0:x})", Marshal.GetLastWin32Error()));
            }

            // CertDeleteCertificateFromStore frees the hContext when it is done

            // clean up store handle
            NativeMethods.CertCloseStore(hCertStore, 0);
        }
    }
}
