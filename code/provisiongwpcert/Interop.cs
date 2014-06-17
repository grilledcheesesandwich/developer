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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;

namespace ProvisionGwpCert
{
    public static class NativeMethods
    {
        // return values, add if needed
        public const int S_OK = 0;
        public const int S_FALSE = 0x1;
        public const uint ERROR_SUCCESS = 0;
        public const int E_INSUFFICIENT_BUFFER = unchecked((int)0x8007007A);
        public const int E_INVALIDARG = unchecked((int)0x80070057);

        public static bool Failed(int hr)
        {
            return hr < 0;
        }

        #region GWPCPC
        private const int GWPCPC_ERROR_BASE = unchecked((int)0x81120000);
        public const int GWPCPC_E_BAD_DEVICE_DATA                  = GWPCPC_ERROR_BASE + 1;
        public const int GWPCPC_E_ALREADY_PROVISION_CERT           = GWPCPC_ERROR_BASE + 2;
        public const int GWPCPC_E_INVALID_ACTIVATION_CODE          = GWPCPC_ERROR_BASE + 3;
        public const int GWPCPC_E_DISABLED_ACTIVATION_CODE         = GWPCPC_ERROR_BASE + 4;
        public const int GWPCPC_E_REDEEMED_ACTIVATION_CODE         = GWPCPC_ERROR_BASE + 5;
        public const int GWPCPC_E_SERVICE_UNAVAILABLE              = GWPCPC_ERROR_BASE + 6;
        public const int GWPCPC_E_SERVICE_UNAVAILABLE_CHECK_TIME   = GWPCPC_ERROR_BASE + 7;
        public const int GWPCPC_E_TIMEOUT                          = GWPCPC_ERROR_BASE + 8;
        public const int GWPCPC_E_SERVER_INTERNAL_ERROR            = GWPCPC_ERROR_BASE + 9;
        public const int GWPCPC_E_SERVER_BUSY                      = GWPCPC_ERROR_BASE + 0xA;
        public const int GWPCPC_E_BAD_REQUEST                      = GWPCPC_ERROR_BASE + 0xB;
        public const int GWPCPC_E_BAD_RESPONSE                     = GWPCPC_ERROR_BASE + 0xC;
        public const int GWPCPC_E_FAIL_INSTALL_CERT                = GWPCPC_ERROR_BASE + 0xD;
        public const int GWPCPC_E_FAIL_GENERATE_KEY                = GWPCPC_ERROR_BASE + 0xE;
        public const int GWPCPC_E_CLIENT_CONFIG_ERROR              = GWPCPC_ERROR_BASE + 0xF;
        public const int E_UNEXPECTED = unchecked((int)0x8000ffff);
        #endregion

        #region GwpCPC

        [DllImport("GwpCPC.dll")]
        static internal extern int GwpCertProvisionCreate([Out] out IntPtr phGwpCP);

        [DllImport("GwpCPC.dll")]
        static internal extern int GwpCertProvisionPrepare([In] IntPtr hGwpCP);

        [DllImport("GwpCPC.dll", CharSet = CharSet.Unicode)]
        static internal extern int GwpCertProvisionCommit([In] IntPtr phGwpCP, [In] string wszActivationCode);

        [DllImport("GwpCPC.dll")]
        static internal extern int GwpCertProvisionClose([In] IntPtr hGwpCP);

        #endregion

        [DllImport("crypt32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CertGetEnhancedKeyUsage(
            IntPtr pCertContext,
            uint dwFlags,
            IntPtr pUsage,
            out uint pcbUsage);

        [StructLayout(LayoutKind.Sequential)]
        internal struct CERT_ENHKEY_USAGE
        {
            public uint cUsageIdentifier;
            public IntPtr rgpszUsageIdentifier;
        }
    }
}

