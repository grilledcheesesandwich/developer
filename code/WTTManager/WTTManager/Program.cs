using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DistributedAutomation;
using Microsoft.DistributedAutomation.Asset;
using Microsoft.DistributedAutomation.Diagnostics;
using Microsoft.DistributedAutomation.ComponentHierarchy;
using Microsoft.DistributedAutomation.Jobs;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.DistributedAutomation.Mobile;
//using Microsoft.Internal.Tools.Wtt.WttModel;


namespace WTTManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverName = "WPCWTTBSQL01";
            string dbName = "WindowsPhone_Blue";

            string connectionString = String.Format("SERVER = {0}; DATABASE = {1}; Integrated Security = SSPI; Pooling = true;",
                serverName,
                dbName);
            
            var wttData = new Microsoft.Internal.Tools.Wtt.WttModel.WttDataContext(connectionString);

            var jobs = wttData.Jobs.Where(j => j.AssignedToAlias == Environment.UserName);
            foreach (var job in jobs)
            {
                //job.CommonContext.IsInactive = true;
                //wttData.SubmitChanges();
                Console.WriteLine("{0}{1}: {2}", job.Id, job.CommonContext.IsInactive ? " (Disabled)" : "", job.Name);
                
                foreach (var jobCategory in job.JobCategories)
                {
                    Console.WriteLine("\t\t{0}{1}", jobCategory.Category.Path, jobCategory.Category.Name);
                }
                foreach (var constraint in job.CommonContext.Constraints)
                {
                    Console.WriteLine("\t{0} {1} {2}", constraint.Dimension.Name, constraint.QueryOperator.DisplayName, String.Join(", ", constraint.ConstraintValues.Select(val => val.ConstraintVal)));
                }
            }

            Console.ReadKey();
        }

        static void Main1(string[] args)
        {
            //var jobs = WTTExecutor.GetJobsFromIDs(new List<string> { "116284", "112514", "113754" });
            var jobs = WTTExecutor.GetJobsFromPath(@"$\CorePlat\CoreOS\CSI\SecurityModel\Device");
            //var jobs = WTTExecutor.GetJobsFromWtq(@"\\winphonelabs\securestorage\TestExecuter\02122013\unionp\000213\wtq.wtq");
            //var jobs = WTTExecutor.GetJobsAssignedTo("unionp");
            string outputFile = "WTTManagerOutput.txt";
            using (StreamWriter sw = new StreamWriter(outputFile))
            {
                foreach (Job job in jobs)
                {
                    string jobOutput = String.Format("{0,7}\t{1}\\{2}", job.Id, job.FullPath, job.Name);
                    Console.WriteLine(jobOutput);
                    sw.WriteLine(jobOutput);
                }
            }
            System.Diagnostics.Process.Start(outputFile);

            //return;

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
            var dataStore = Enterprise.Connect("WindowsPhone_Blue", JobsDefinitionDataStore.ServiceName, identityInfo);

            // Get all JobCategories
            Query jobCategoryQuery = new Query(typeof(JobCategory));
            jobCategoryQuery.AddExpression("CategoryId", QueryOperator.Equals, 6839);
            JobCategoryCollection jobCategories = dataStore.Query(jobCategoryQuery) as JobCategoryCollection;

            // Get all possible descriptors
            Query descriptorQuery = new Query(typeof(Descriptor));
            DescriptorCollection descriptorsQueryResults = dataStore.Query(descriptorQuery) as DescriptorCollection;
            List<Descriptor> allDescriptors = new List<Descriptor>(descriptorsQueryResults.Count);
            foreach (Descriptor descriptor in descriptorsQueryResults)
            {
                allDescriptors.Add(descriptor);
            }

            //// Get all possible constraints
            //Query constraintQuery = new Query(typeof(Constraint));
            //ConstraintCollection constraintQueryResults = dataStore.Query(constraintQuery) as ConstraintValueCollection;
            //List<Constraint> allConstraints = new List<Constraint>(constraintQueryResults.Count);
            //foreach (Constraint constraint in constraintQueryResults)
            //{
            //    allConstraints.Add(constraint);
            //}

            //// Use wtq query
            string filename = @"\\winphonelabs\securestorage\TestExecuter\02122013\unionp\000213\wtq.wtq";
            Query jobQuery;
            WtqHelper wtqHelper = new WtqHelper(filename);
            jobQuery = wtqHelper.GetQuery(dataStore, typeof(Job));

            // Get jobs
            //Query jobQuery = new Query(typeof(Job));
            //jobQuery.AddSortColumn("FullPath", SortDirection.Ascending);
            //jobQuery.AddExpression("Id", QueryOperator.Equals, "112514");
            //jobQuery.AddExpression("Priority", QueryOperator.EqualsLess, 1);
            //jobQuery.AddConjunction(Conjunction.And);
            //jobQuery.AddExpression("FullPath", QueryOperator.BeginsWith, @"$\OS Platform\CoreOS\Security\");
            //jobQuery.AddConjunction(Conjunction.And);

            // Version Attribute
            {
                //jobQuery.AddConjunction(Conjunction.And);
                //QueryStatement attributeStatement = new QueryStatement();
                //attributeStatement.AddExpression("FullPath", QueryOperator.Contains, @"$\Version\7\Mainline");
                //jobQuery.AddExpression("DescriptorCollection", QueryOperator.Has, attributeStatement);
            }

            //// (x)BVT Attribute
            //{
            //    jobQuery.AddConjunction(Conjunction.And);
            //    QueryStatement attributeStatement = new QueryStatement();
            //    attributeStatement.AddExpression("FullPath", QueryOperator.Contains, @"$\Type\BVT\Buildlab_xBVT");
            //    jobQuery.AddExpression("DescriptorCollection", QueryOperator.Has, attributeStatement);
            //}

            // Suite\Test attribute
            {
                //jobQuery.AddConjunction(Conjunction.And);
                //QueryStatement attributeStatement = new QueryStatement();
                //attributeStatement.AddExpression("FullPath", QueryOperator.Contains, @"$\Suite\Test");
                //jobQuery.AddExpression("DescriptorCollection", QueryOperator.HasNot, attributeStatement);
            }

            //string outputFile = "WTTManagerOutput.txt";

            //Regex rgxCmdLine = new Regex(@"(?<=CommandLine="")[^""]*", RegexOptions.Compiled);

            //using (StreamWriter sw = new StreamWriter(outputFile))
            //{
            //    JobCollection jobs = dataStore.Query(jobQuery) as JobCollection;
            //    foreach (Job job in jobs)
            //    {
            //        string jobOutput = String.Format("{0,7}\t{1}\\{2}", job.Id, job.FullPath, job.Name);
            //        Console.WriteLine(jobOutput);
            //        sw.WriteLine(jobOutput);
            //        //Console.WriteLine(job.Description);

            //        // Get tasks
            //        try
            //        {
            //            foreach (Task task in job.TaskCollection)
            //            {
            //                string temp = rgxCmdLine.Match(task.TaskXml).Value;
            //                temp = System.Web.HttpUtility.HtmlDecode(temp);
            //                string[] lines = temp.Split('\n');
            //                foreach (string line in lines)
            //                {
            //                    string formatted = String.Format("\t{0}", line.Trim());
            //                    //Console.WriteLine(formatted);
            //                    //sw.WriteLine(formatted);
            //                }
            //            }
            //        }
            //        catch { Console.WriteLine("ERROR"); }

            //        // Get attributes
            //        {
            //            int[] descriptorIdArray = new int[job.CommonContext.ContextDescriptorCollection.Count];
            //            for (int index = 0; index < job.CommonContext.ContextDescriptorCollection.Count; index++)
            //            {
            //                descriptorIdArray[index] = job.CommonContext.ContextDescriptorCollection[index].DescriptorId;
            //            }

            //            //int i = 0;
            //            foreach (var descriptor in allDescriptors.Where(descriptor => descriptorIdArray.Contains(descriptor.Id)))
            //            {
            //                //if (descriptor.FullPath.EndsWith("Suite"))
            //                //{
            //                //    break;
            //                //}
            //                //i++;

            //                //Console.WriteLine("\t{0}", descriptor.FullPath);
            //                //sw.WriteLine("\t{0}", descriptor.FullPath);
            //            }
            //            //if (i < job.CommonContext.ContextDescriptorCollection.Count)
            //            //{
            //            //    job.CommonContext.ContextDescriptorCollection[i].DataStoreOperation = DataStoreOperation.Delete;
            //            //    job.CommitToDataStore();
            //            //}
            //        }

            //        // Get parameters
            //        {
            //            var parameters = job.CommonContext.ParameterCollection;
            //            foreach (Parameter param in parameters)
            //            {
            //                Console.WriteLine("\t{0}: {1}", param.Name, param.ParameterVal);
            //                sw.WriteLine("\t{0}: {1}", param.Name, param.ParameterVal);
            //            }

            //            //int[] paraeteIdArray = new int[job.CommonContext.ContextDescriptorCollection.Count];
            //            //for (int index = 0; index < job.CommonContext.ContextDescriptorCollection.Count; index++)
            //            //{
            //            //    descriptorIdArray[index] = job.CommonContext.ContextDescriptorCollection[index].DescriptorId;
            //            //}
            //        }

            //        //// Get constraints
            //        //{
            //        //    int[] constraintIdArray = new int[job.CommonContext.ConstraintCollection.Count];
            //        //    for (int index = 0; index < job.CommonContext.ConstraintCollection.Count; index++)
            //        //    {
            //        //        constraintIdArray[index] = job.CommonContext.ConstraintCollection[index].DimensionId;
            //        //    }

            //        //    foreach (var constraint in allConstraints.Where(constraint => constraintIdArray.Contains(constraint.Id)))
            //        //    {
            //        //        Console.WriteLine("\t{0}", constraint.FullPath);
            //        //        sw.WriteLine("\t{0}", descriptor.FullPath);
            //        //    }
            //        //}
            //    }
            //}
            //System.Diagnostics.Process.Start(outputFile);
        }
    }
}