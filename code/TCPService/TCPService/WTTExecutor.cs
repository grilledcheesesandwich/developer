using Microsoft.DistributedAutomation;
using Microsoft.DistributedAutomation.Jobs;
using Microsoft.DistributedAutomation.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiService
{
    class WTTExecutor
    {
        private static DataStore dataStore;

        static WTTExecutor()
	    {
            // Parameters for the WTT SqlIdentityConnectInfo object.
            // It seems unlikely these will change, but put them in variables just in case.
            //
            // Controller hosting the identity service
            string serverName = "WPCWTTBID01.redmond.corp.microsoft.com";
            // DB name for the identity service
            string dbName = "WTTIdentity";

            // 1.  Specify how to connect to the Identity service for the WTT Enterprise.  This information is a constant in the enterprise.
            var identityInfo = new SqlIdentityConnectInfo(serverName, dbName);

            // 2.  Connect to the JobsDefinition service of the datastore we are interested in.
            dataStore = Enterprise.Connect("WindowsPhone_Blue", JobsDefinitionDataStore.ServiceName, identityInfo);
	    }

        public static IEnumerable<Job> GetJobsFromWtq(string path)
        {
            Microsoft.DistributedAutomation.Query jobQuery;
            WtqHelper wtqHelper = new WtqHelper(path);
            jobQuery = wtqHelper.GetQuery(dataStore, typeof(Job));
            return ExecuteQuery(jobQuery);
        }

        public static IEnumerable<Job> GetJobsFromResultCollection(string path)
        {
            Microsoft.DistributedAutomation.Query resultQuery;
            WtqHelper wtqHelper = new WtqHelper(path);
            resultQuery = wtqHelper.GetQuery(dataStore, typeof(Result));
            return ExecuteQuery(resultQuery);
        }

        public static Job GetJobFromID(string jobID)
        {
            var jobQuery = new Microsoft.DistributedAutomation.Query(typeof(Job));
            jobQuery.AddExpression("Id", QueryOperator.Equals, jobID);

            return ExecuteQuery(jobQuery).Single();
        }

        public static IEnumerable<Job> GetJobsFromIDs(List<string> jobIDs)
        {
            //Query jobQuery = new Query(typeof(Job));
            //for (int i = 0; i < jobIDs.Count - 1; i++)
            //{
            //    jobQuery.AddExpression("Id", QueryOperator.Equals, jobIDs[i]);
            //    jobQuery.AddConjunction(Conjunction.Or);
            //}
            //jobQuery.AddExpression("Id", QueryOperator.Equals, jobIDs.Last());

            //return ExecuteQuery(jobQuery);

            foreach (var jobID in jobIDs)
            {
                yield return GetJobFromID(jobID);
            }
        }

        public static IEnumerable<Job> GetJobsFromPath(string path)
        {
            var jobQuery = new Microsoft.DistributedAutomation.Query(typeof(Job));
            jobQuery.AddExpression("FullPath", QueryOperator.BeginsWith, path);
            return ExecuteQuery(jobQuery);
        }

        private static IEnumerable<Job> ExecuteQuery(Microsoft.DistributedAutomation.Query query)
        {
            var queryResult = dataStore.Query(query);
            if (queryResult is JobCollection)
            {
                var jobs = queryResult as JobCollection;
                foreach (Job job in jobs)
                {
                    yield return job;
                }
            }
            else if (queryResult is ResultSummaryCollection)
            {
                var resultSummaryCollection = queryResult as ResultSummaryCollection;
                foreach (ResultSummary resultSummary in resultSummaryCollection)
                {
                    var resultCollection = Microsoft.Internal.Tools.Wtt.WttUtility.GetResults(dataStore, resultSummary.Id);
                    var results = new HashSet<string>();
                    foreach (Result result in resultCollection)
                    {
                        if (result.ParentTaskId == Int32.MinValue)
                        {
                            // These aren't template jobs
                            results.Add(result.JobId.ToString());
                        }
                    }
                    foreach (string jobId in results)
                    {
                        yield return GetJobFromID(jobId);
                    }
                }
            }
            else
            {
                throw new Exception("Invalid query");
            }
        }

        public static IEnumerable<string> GetPackagesForJobs(IEnumerable<Job> jobs)
        {
            List<string> packages = new List<string>();
            foreach (Job job in jobs)
            {
                foreach (Parameter parameter in job.CommonContext.ParameterCollection)
                {
                    if (parameter.Name.Equals("system_TestPackages", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (parameter.ParameterVal != null)
                        {
                            packages.AddRange(parameter.ParameterVal.Split(';'));
                        }
                    }
                }
            }
            return packages.Distinct(StringComparer.InvariantCultureIgnoreCase);
        }
    }
}
