using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PEC
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                int number;
                if (args[0].StartsWith("0x"))
                {
                    int.TryParse(args[0].Substring(2), NumberStyles.AllowHexSpecifier, null, out number);
                }
                else
                {
                    int.TryParse(args[0], out number);
                }

                Console.WriteLine(NativeMethods.GetErrorMessage(number));
            }
        }
    }

    class NativeMethods
    {
        [DllImport("ntdll.dll")]
        public static extern int RtlNtStatusToDosError(int Status);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int FormatMessage(int dwFlags,
                                                IntPtr lpSource,
                                                int dwMessageId,
                                                int dwLanguageId,
                                                StringBuilder lpBuffer,
                                                int nSize,
                                                IntPtr va_list_arguments);

        private const int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        private const int ERROR_MR_MID_NOT_FOUND = 317;

        /// <summary>
        /// Gets a formatted message containing a description of the specified Win32 error code.
        /// </summary>
        /// <param name="errorCode">The Win32 error code</param>
        /// <returns></returns>
        public static string GetErrorMessage(int errorCode)
        {
            StringBuilder buffer = new StringBuilder(1024);
            string message = "Unknown error";
            int messageLength = 0;

            messageLength = NativeMethods.FormatMessage(NativeMethods.FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, errorCode, 0, buffer, buffer.Capacity, IntPtr.Zero);

            if (messageLength == 0)
            {
                int dosError = NativeMethods.RtlNtStatusToDosError(errorCode);

                if (dosError != ERROR_MR_MID_NOT_FOUND)
                {
                    messageLength = NativeMethods.FormatMessage(NativeMethods.FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, dosError, 0, buffer, buffer.Capacity, IntPtr.Zero);
                }
            }

            if (messageLength > 0)
            {
                //
                // Trim off any trailing whitespace (Win32 error messages tend to end in a CR/LF
                // sequence).
                //

                message = buffer.ToString(0, messageLength).Trim();

                //
                // If the message ends in a period, remove it for consistency with the CLR's
                // message formatting.
                //

                if (message.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    message = message.Substring(0, message.Length - 1);
                }
            }

            return message;
        }
    }
}
