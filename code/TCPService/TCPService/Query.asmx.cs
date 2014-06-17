using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace MultiService
{
    /// <summary>
    /// Summary description for Query
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Query : System.Web.Services.WebService
    {
        [WebMethod]
        public List<string> GetJobsFromWtqXml(string wtqXml)
        {
            string localPath = Path.GetTempFileName() + ".wtq";
            File.WriteAllText(localPath, wtqXml);
            return GetJobsFromWtq(localPath);
        }

        [WebMethod]
        public List<string> GetJobsFromWtq(string wtqPath)
        {
            var jobs = WTTExecutor.GetJobsFromWtq(wtqPath);
            return jobs.Select(job => job.Id.ToString()).ToList();
        }

        [WebMethod]
        public List<string> GetJobsFromResultCollection(string wtqPath)
        {
            var jobs = WTTExecutor.GetJobsFromResultCollection(wtqPath);
            return jobs.Select(job => job.Id.ToString()).ToList();
        }

        [WebMethod]
        public List<string> GetJobNames(List<string> jobIDs)
        {
            return WTTExecutor.GetJobsFromIDs(jobIDs).Select(job => job.Name).ToList();
        }

        [WebMethod]
        public string GetDpkNameFromDpkBytes(byte[] dpkBytes)
        {
            return "foo";
        }

        [WebMethod]
        public string GetDpkName(string dpkPath)
        {
            ProcessStartInfo psi = new ProcessStartInfo(@"c:\sdpack\sdpack.bat", String.Format("describe \"{0}\" -D", dpkPath));
            psi.WorkingDirectory = @"c:\sdpack";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            //psi.WindowStyle = ProcessWindowStyle.Hidden;
            //psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            using (Process p = Process.Start(psi))
            {
                p.WaitForExit();
                string error = p.StandardError.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return error;
                }
                else
                {
                    return p.StandardOutput.ReadToEnd().Split('\n')[2].Trim();
                }
            }
        }

        [WebMethod]
        public string GetIdentity()
        {
            return User.Identity.Name;
        }
    }
}
