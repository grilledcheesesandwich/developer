using System;

using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using Microsoft.MobileDevices.AreaLibrary.Connectivity;

namespace ToggleAdvanced
{
    class Program
    {
        static void Main(string[] args)
        {
            if (CellularConnectionWizard.IsAdvancedDialogEnabledInRegistry())
            {
                Console.WriteLine("Disabling all advanced settings");
                CellularConnectionWizard.CleanCellularRegKey();
            }
            else
            {
                Console.WriteLine("Enabling all advanced settings");
                CellularConnectionWizard.EnableAllAdvancedSettings();
            }
        }
    }
}
