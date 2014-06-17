using Microsoft.DistributedAutomation;
using Microsoft.DistributedAutomation.Jobs;
using Microsoft.DistributedAutomation.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WTTManager
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

        public static JobCollection GetJobsFromWtq(string path)
        {
            Query jobQuery;
            WtqHelper wtqHelper = new WtqHelper(path);
            jobQuery = wtqHelper.GetQuery(dataStore, typeof(Job));
            return ExecuteQuery(jobQuery);
        }

        public static JobCollection GetJobsFromIDs(List<string> jobIDs)
        {
            Query jobQuery = new Query(typeof(Job));
            for (int i = 0; i < jobIDs.Count - 1; i++)
            {
                jobQuery.AddExpression("Id", QueryOperator.Equals, jobIDs[i]);
                jobQuery.AddConjunction(Conjunction.Or);
            }
            jobQuery.AddExpression("Id", QueryOperator.Equals, jobIDs.Last());

            return ExecuteQuery(jobQuery);
        }

        public static JobCollection GetJobsFromPath(string path)
        {
            Query jobQuery = new Query(typeof(Job));
            jobQuery.AddExpression("FullPath", QueryOperator.BeginsWith, path);
            return ExecuteQuery(jobQuery);
        }

        public static JobCollection GetJobsAssignedTo(string alias)
        {
            Query jobQuery = new Query(typeof(Job));
            
            jobQuery.AddExpression("AssignedToAlias", QueryOperator.Equals, alias);
            return ExecuteQuery(jobQuery);
        }

        private static JobCollection ExecuteQuery(Query jobQuery)
        {
            return dataStore.Query(jobQuery) as JobCollection;
        }
    }
}
