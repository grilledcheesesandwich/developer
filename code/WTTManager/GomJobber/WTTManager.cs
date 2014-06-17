using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Internal.Tools.Wtt.WttModel;

namespace GomJobber
{
    public class JobStatus
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Disabled { get; set; }
        public IEnumerable<string> Categories { get; set; }
        public IEnumerable<Constraint> Constraints { get; set; }
    }

    class WTTManager
    {
        const string serverName = "WPCWTTBSQL01";
        const string dbName = "WindowsPhone_Blue";
        WttDataContext wttData;

        public WTTManager()
        {
            string connectionString = String.Format("SERVER = {0}; DATABASE = {1}; Integrated Security = SSPI; Pooling = true;",
                serverName,
                dbName);

            wttData = new WttDataContext(connectionString);
        }

        public IEnumerable<JobStatus> GetJobStatuses(string alias)
        {
            var jobs = wttData.Jobs.Where(j => j.AssignedToAlias == alias);
            foreach (var job in jobs)
            {
                var status = new JobStatus();
                status.ID = job.Id;
                status.Name = job.Name;
                status.Disabled = job.CommonContext.IsInactive;
                status.Constraints = job.CommonContext.Constraints;
                status.Categories = job.JobCategories.Select(c => c.Category.Name);
                yield return status;
            }
        }
    }
}
