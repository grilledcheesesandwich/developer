using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace RELFSDParser
{
    public partial class frmMain : Form
    {
        List<string> files = new List<string>();
        string path;
        bool alreadyConfiguredRapi = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == ofdLog.ShowDialog())
            {
                ParseFile(ofdLog.FileName);               
            }
        }

        private void ParseFile(string logFile)
        {
            using (StreamReader sr = new StreamReader(logFile))
            {
                Parse(sr.ReadToEnd());
            }
        }

        private void Parse(string log)
        {
            txtFiles.Clear();
            files.Clear();
            using (StringReader sr = new StringReader(log))
            {
                while (-1 != sr.Peek())
                {
                    string line = sr.ReadLine();
                    if (-1 != line.IndexOf("RELFSD"))
                    {
                        int start = line.IndexOf("Opening file") + 13;
                        int length = (line.IndexOf("from desktop") - 1) - start;
                        string file = line.Substring(start, length);
                        if ((!file.StartsWith("VD_")) && (!file.EndsWith("log")) && (!files.Contains(file)))
                            files.Add(file.Replace(path, ""));
                    }
                }
            }
            if (0 == files.Count)
            {
                txtFiles.Text = "The log doesn't show any files copied over RELFSD.  Results[x].log files don't show RELFSD calls, you need to copy the output from PB into a .log file and open that instead.";
                btnCopy.Enabled = false;
                btnCopyDevice.Enabled = false;
                return;
            }
            foreach (string file in files)
            {
                txtFiles.Text += file + '\n';
            }
            btnCopy.Enabled = true;
            btnCopyDevice.Enabled = true;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            fbdDirectory.Description = "Destination directory";
            if (DialogResult.OK == fbdDirectory.ShowDialog())
            {
                int filesCopied = CopyFilesToLocalFolder(fbdDirectory.SelectedPath);
                MessageBox.Show("Copied " + filesCopied + " files", "Success!");
            }
        }

        private int CopyFilesToLocalFolder(string destPath)
        {
            int filesCopied = 0;

            foreach (string filePath in txtFiles.Lines)
            {
                if ("" == filePath) continue;

                string file = filePath.TrimStart('\\');

                string destination = Path.Combine(destPath, file);

                if (-1 != file.IndexOf('\\'))
                {
                    string dir = Path.Combine(destPath, file.Remove(file.LastIndexOf('\\')));
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }

                if (File.Exists(destination))
                {
                    File.SetAttributes(destination, FileAttributes.Normal);
                }
                File.Copy(Path.Combine(path, file), destination, true);
                ++filesCopied;
            }

            return filesCopied;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            path = Environment.GetEnvironmentVariable("_FLATRELEASEDIR");

            if (null == path)
            {
                MessageBox.Show("You need to run this from a build window", "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            ofdLog.InitialDirectory = path;

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
            {
                ParseFile(args[1]);
            }

            txtFrom.Text = path;
            txtTo.Text = @"\test";
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
                Parse(Clipboard.GetText());
        }

        private void btnCopyDevice_Click(object sender, EventArgs e)
        {
            string ceCopyPath = Path.Combine(Environment.GetEnvironmentVariable("_WINCEROOT"), @"private\mdd\bin\i386\cecopy.exe");
            //string rapiStartPath = Path.Combine(Environment.GetEnvironmentVariable("_WINCEROOT"), @"private\mdd\bin\i386\rapistart.exe");

            //List<string> commands = new List<string>();
            //List<string> dirs = new List<string>();
            //int filesCopied = 0;

            //dirs.Add(@"\test");

            //if (!alreadyConfiguredRapi)
            //{
                //Process.Start(rapiCopyPath, @"rapiunlock.cab \rapiunlock.cab");
                //Process.Start(rapiStartPath, @"wceload \rapiunlock.cab");
                //alreadyConfiguredRapi = true;
            //}

            //foreach (string f in txtFiles.Lines)
            //{
            //    if ("" == f) continue;

            //    string file = f.TrimStart('\\');

            //    if (-1 != file.IndexOf('\\'))
            //    {
            //        string dir = Path.Combine(@"\test", file.Remove(file.LastIndexOf('\\')));
            //        if (!dirs.Contains(dir))
            //        {
            //            dirs.Add(dir);
            //        }
            //    }

            //    string source = Path.Combine(path, file);
            //    string destination = Path.Combine(@"\test", file);

            //    string command = source + " " + destination;

            //    if (!commands.Contains(command))
            //    {
            //        commands.Add(command);
            //        ++filesCopied;
            //    }
            //}

            //foreach (string dir in dirs)
            //{
            //    MakeDirOnDevice(dir);
            //}

            //foreach (string command in commands)
            //{
            //    using (Process p = Process.Start(rapiCopyPath, command))
            //    {
            //        p.WaitForExit();
            //    }
            //}

            string tempDir = Path.Combine(Environment.GetEnvironmentVariable("temp"), "RELFSDParser");
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);

            CopyFilesToLocalFolder(tempDir);

            using (Process p = Process.Start(ceCopyPath, "/s " + tempDir + @" dev:" + txtTo.Text))
            {
                p.WaitForExit();
            }
            //arguments = arguments.TrimEnd('&') + "\"";
            
            //MessageBox.Show("Copied " + filesCopied + " files", "Success!");
        }

        private void MakeDirOnDevice(string path)
        {
            string rapiConfigPath = Path.Combine(Environment.GetEnvironmentVariable("_WINCEROOT"), @"private\mdd\bin\i386\rapiconfig.exe");

            using (StreamWriter sw = new StreamWriter("makedir.xml"))
            {
                string xml = String.Format(@"<wap-provisioningdoc>
    <characteristic type=""FileOperation"">
        <characteristic type=""{0}"" translation=""install"">
            <characteristic type=""MakeDir"" />
        </characteristic>
    </characteristic>
</wap-provisioningdoc>", path);
                sw.Write(xml);
            }
            using (Process p = Process.Start(rapiConfigPath, @"makedir.xml"))
            {
                p.WaitForExit();
            }
            File.Delete("makedir.xml");
        }
    }
}