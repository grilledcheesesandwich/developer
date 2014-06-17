using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Management;
using System.IO;
using System.Xml.Serialization;

namespace LogonTracker
{
    static class Program
    {
        private static string AppDir { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogonTracker"); } }
        private static string LogonFilename { get { return Path.Combine(AppDir, "logons.dat"); } }
        private static string LogonCsvFilename { get { return Path.Combine(AppDir, "logons.csv"); } }
        private static List<DateTime> logons;
        private static NotifyIcon ni;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SystemEvents.SessionSwitch += new SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            ni = new NotifyIcon();
            ni.Icon = new System.Drawing.Icon(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("LogonTracker.login.ico"));
            ni.DoubleClick += (o, a) => Application.Exit();
            ni.Visible = true;

            UpdateLogonTime();

            Application.Run();

            ni.Dispose();
        }

        private static void UpdateLogonTime()
        {
            // If it's before 6am, then I'm probably just working past midnight
            if (DateTime.Now.Hour < 6) return;

            if (logons == null)
            {
                logons = Load(LogonFilename);
                logons.Sort();
            }
            if (logons.Count == 0 || logons.Last().Date != DateTime.Now.Date)
            {
                logons.Add(DateTime.Now);
                Save(logons, LogonFilename);
            }
            ni.Text = String.Format("First logon today: {0}", logons.Last());
        }

        static void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                UpdateLogonTime();
            }
        }

        public static void Save(List<DateTime> list, string fileName)
        {
            //create a backup
            string backupName = Path.ChangeExtension(fileName, ".old");
            if (File.Exists(fileName))
            {
                if (File.Exists(backupName)) File.Delete(backupName);
                File.Move(fileName, backupName);
            }

            if (!Directory.Exists(AppDir)) Directory.CreateDirectory(AppDir);

            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<DateTime>));
                ser.Serialize(fs, list);
            }
            using (StreamWriter sw = new StreamWriter(LogonCsvFilename))
            {
                Console.WriteLine("Date,Time");
                list.ForEach(dt => sw.WriteLine("{0}/{1}/{2},{3}", dt.Month, dt.Day, dt.Year, dt.ToShortTimeString()));
            }
        }

        public static List<DateTime> Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return new List<DateTime>();
            }

            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<DateTime>));
                return (List<DateTime>)ser.Deserialize(fs);
            }
        }
    }
}
