using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetClip
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                SetText(args[0]);
            }
            else
            {
                string piped = Console.ReadLine();
                if (!String.IsNullOrEmpty(piped))
                {
                    SetText(piped);
                }
            }
        }

        static void SetText(string text)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    System.Windows.Forms.Clipboard.SetText(text);
                    return;
                }
                catch (Exception) { }
            }
        }
    }
}
